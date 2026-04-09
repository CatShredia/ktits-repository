using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Hubs;


[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly DatabaseContext _context;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(DatabaseContext context, ILogger<ChatHub> logger)
    {
        _context = context;
        _logger = logger;
    }


    // ! JoinConversation - adds user to SignalR conversation group
    // вызывается из CinemaBlazor через ChatHubService.JoinConversationAsync
    public async Task JoinConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            await Clients.Caller.UserDisconnected(conversationId, -1, "Unauthorized");
            return;
        }

        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            _logger.LogWarning("User {UserId} tried to join conversation {ConversationId} without permission",
                userId, conversationId);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetConversationGroupName(conversationId));

        var user = await _context.Users.FindAsync(userId.Value);
        if (user != null)
        {
            await Clients.GroupExcept(GetConversationGroupName(conversationId), Context.ConnectionId)
                .UserConnected(conversationId, userId.Value, $"{user.Surname} {user.Name}");
        }

        _logger.LogInformation("User {UserId} joined conversation {ConversationId}", userId.Value, conversationId);
    }


    // ! LeaveConversation - removes user from SignalR conversation group
    // вызывается из CinemaBlazor через ChatHubService.LeaveConversationAsync
    public async Task LeaveConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetConversationGroupName(conversationId));

        var user = await _context.Users.FindAsync(userId.Value);
        if (user != null)
        {
            await Clients.Group(GetConversationGroupName(conversationId))
                .UserDisconnected(conversationId, userId.Value, $"{user.Surname} {user.Name}");
        }

        _logger.LogInformation("User {UserId} left conversation {ConversationId}", userId.Value, conversationId);
    }


    // ! SendMessageToConversation - sends message to all users in conversation via SignalR
    // откуда вызывается: вызывается из CinemaBlazor через ChatHubService.SendMessageAsync
    [Microsoft.AspNetCore.SignalR.HubMethodName("sendMessageToConversation")]
    public async Task SendMessageToConversation(int conversationId, string content)
    {
        Console.WriteLine($"[HUB] sendMessageToConversation called! convId={conversationId}, content={content}");
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        // Получаем участника с ролью и тип чата
        var participant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null)
        {
            _logger.LogWarning("User {UserId} tried to send message to conversation {ConversationId} without permission",
                userId, conversationId);
            return;
        }

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conversation == null) return;

        // Проверка прав на отправку сообщений
        if (!RolePermissions.CanSendMessage(participant.Role.Name, conversation.ConversationType.Name))
        {
            _logger.LogWarning("User {UserId} tried to send message to conversation {ConversationId} without permission (role: {Role})",
                userId, conversationId, participant.Role.Name);
            return;
        }

        if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
        {
            return;
        }

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = userId.Value,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(userId.Value);
        if (sender == null)
        {
            return;
        }

        var messageResponse = new MessageResponse
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = $"{sender.Surname} {sender.Name}",
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };

        await Clients.Group(GetConversationGroupName(conversationId))
            .ReceiveMessage(messageResponse);

        _logger.LogInformation("User {UserId} sent message to conversation {ConversationId}", userId, conversationId);
    }


    // ! UserIsTyping - notifies other users that current user is typing
    // вызывается из CinemaBlazor через ChatHubService.SendTypingAsync
    public async Task UserIsTyping(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            return;
        }

        var user = await _context.Users.FindAsync(userId.Value);
        if (user != null)
        {
            await Clients.GroupExcept(GetConversationGroupName(conversationId), Context.ConnectionId)
                .UserTyping(conversationId, userId.Value, $"{user.Surname} {user.Name}");
        }
    }


    // ! DeleteMessage - deletes message from conversation based on role permissions
    // вызывается из CinemaBlazor через ChatHubService.DeleteMessageAsync
    public async Task DeleteMessage(int messageId, int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        var message = await _context.Messages.FindAsync(messageId);
        if (message == null || message.ConversationId != conversationId)
        {
            return;
        }

        // Получаем участника с ролью
        var participant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return;

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conversation == null) return;

        // Проверка прав на удаление
        var isOwnMessage = message.SenderId == userId.Value;
        bool canDelete;

        if (isOwnMessage)
        {
            canDelete = RolePermissions.CanDeleteOwnMessage(participant.Role.Name, conversation.ConversationType.Name);
        }
        else
        {
            canDelete = RolePermissions.CanDeleteOtherMessage(participant.Role.Name);
        }

        if (!canDelete)
        {
            _logger.LogWarning("User {UserId} tried to delete message {MessageId} without permission (role: {Role}, own: {isOwn})",
                userId.Value, messageId, participant.Role.Name, isOwnMessage);
            return;
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        await Clients.Group(GetConversationGroupName(conversationId))
            .MessageDeleted(messageId, conversationId);

        _logger.LogInformation("User {UserId} deleted message {MessageId} from conversation {ConversationId}",
            userId.Value, messageId, conversationId);
    }

    // ! OnConnectedAsync - called when client connects to SignalR hub
    // вызывается автоматически при подключении клиента к SignalR
    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));
            _logger.LogInformation("User {UserId} connected. ConnectionId: {ConnectionId}",
                userId.Value, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    // ! OnDisconnectedAsync - called when client disconnects from SignalR hub
    // вызывается автоматически при отключении клиента от SignalR
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            _logger.LogInformation("User {UserId} disconnected. ConnectionId: {ConnectionId}",
                userId.Value, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // ! GetCurrentUserId - extracts user ID from JWT token in SignalR context
    // вызывается внутри всех методов этого хаба
    private int? GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    // ! GetConversationGroupName - returns SignalR group name for conversation ID
    // вызывается внутри всех методов отправки сообщений этого хаба
    private string GetConversationGroupName(int conversationId)
    {
        return $"conversation_{conversationId}";
    }

    // ! GetUserGroupName - returns SignalR group name for user ID
    // вызывается внутри OnConnectedAsync метода этого хаба
    private string GetUserGroupName(int userId)
    {
        return $"user_{userId}";
    }
}
