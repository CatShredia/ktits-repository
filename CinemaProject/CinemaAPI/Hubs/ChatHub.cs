using CinemaAPI.Data;
using CinemaAPI.Data.Models;
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

        // Для Comments проверка участника не нужна — доступны всем авторизованным
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null) return;

        if (conversation.ConversationType.Name != "Comments")
        {
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
            {
                _logger.LogWarning("User {UserId} tried to join conversation {ConversationId} without permission",
                    userId, conversationId);
                return;
            }
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

        // Получаем тип чата
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conversation == null) return;

        // Для Comments проверка участника не нужна — доступны всем авторизованным
        ConversationParticipant? participant = null;
        if (conversation.ConversationType.Name != "Comments")
        {
            participant = await _context.ConversationParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null)
            {
                _logger.LogWarning("User {UserId} tried to send message to conversation {ConversationId} without permission",
                    userId, conversationId);
                return;
            }
        }
        else
        {
            // Для Comments — виртуальный участник с ролью Member
            var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");
            participant = new ConversationParticipant { Role = memberRole };
        }

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

        // Для Comments проверка участника не нужна
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null) return;

        if (conversation.ConversationType.Name != "Comments")
        {
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
            {
                return;
            }
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

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conversation == null) return;

        // Для Comments проверка участника не нужна
        ConversationParticipant? participant = null;
        if (conversation.ConversationType.Name != "Comments")
        {
            participant = await _context.ConversationParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null) return;
        }
        else
        {
            // Для Comments — виртуальный участник
            var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");
            participant = new ConversationParticipant { Role = memberRole };
        }

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

    // ! EditMessage - edits message content based on role permissions
    // вызывается из CinemaBlazor через ChatHubService.EditMessageAsync
    public async Task EditMessage(int messageId, int conversationId, string newContent)
    {
        Console.WriteLine($"[HUB EditMessage] START: messageId={messageId}, conversationId={conversationId}, content='{newContent}'");

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            Console.WriteLine($"[HUB EditMessage] FAIL: userId is null");
            return;
        }
        Console.WriteLine($"[HUB EditMessage] userId={userId.Value}");

        var message = await _context.Messages.FindAsync(messageId);
        if (message == null || message.ConversationId != conversationId)
        {
            Console.WriteLine($"[HUB EditMessage] FAIL: message not found or mismatch. message={message?.ToString() ?? "null"}, msg.ConvId={message?.ConversationId}, expected={conversationId}");
            return;
        }
        Console.WriteLine($"[HUB EditMessage] message found: id={message.Id}, convId={message.ConversationId}, senderId={message.SenderId}, content='{message.Content}'");

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conversation == null)
        {
            Console.WriteLine($"[HUB EditMessage] FAIL: conversation {conversationId} not found");
            return;
        }
        Console.WriteLine($"[HUB EditMessage] conversation found: id={conversationId}, type={conversation.ConversationType.Name}");

        // Для Comments проверка участника не нужна
        ConversationParticipant? participant = null;
        if (conversation.ConversationType.Name != "Comments")
        {
            participant = await _context.ConversationParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null)
            {
                Console.WriteLine($"[HUB EditMessage] FAIL: participant not found (convId={conversationId}, userId={userId.Value})");
                return;
            }
            Console.WriteLine($"[HUB EditMessage] participant found, role={participant.Role.Name}");
        }
        else
        {
            // Для Comments — виртуальный участник
            var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");
            participant = new ConversationParticipant { Role = memberRole };
            Console.WriteLine($"[HUB EditMessage] Comments type, virtual participant role=Member");
        }

        // Проверка прав на редактирование
        var isOwnMessage = message.SenderId == userId.Value;
        bool canEdit;

        if (isOwnMessage)
        {
            canEdit = RolePermissions.CanEditOwnMessage(participant.Role.Name, conversation.ConversationType.Name);
            Console.WriteLine($"[HUB EditMessage] own message, canEdit={canEdit}, role={participant.Role.Name}, convType={conversation.ConversationType.Name}");
        }
        else
        {
            canEdit = false; // Редактировать можно только свои сообщения
            Console.WriteLine($"[HUB EditMessage] NOT own message, canEdit=false");
        }

        if (!canEdit)
        {
            Console.WriteLine($"[HUB EditMessage] FAIL: no permission to edit");
            return;
        }

        if (string.IsNullOrWhiteSpace(newContent) || newContent.Length > 1000)
        {
            Console.WriteLine($"[HUB EditMessage] FAIL: content invalid (length={newContent?.Length})");
            return;
        }

        message.Content = newContent;
        message.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        Console.WriteLine($"[HUB EditMessage] message updated in DB");

        var sender = await _context.Users.FindAsync(userId.Value);
        if (sender == null)
        {
            Console.WriteLine($"[HUB EditMessage] FAIL: sender not found");
            return;
        }
        Console.WriteLine($"[HUB EditMessage] sender found: {sender.Surname} {sender.Name}");

        var messageResponse = new MessageResponse
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = $"{sender.Surname} {sender.Name}",
            Content = message.Content,
            ImageUrl = message.ImageUrl,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };

        var groupName = GetConversationGroupName(conversationId);
        Console.WriteLine($"[HUB EditMessage] broadcasting MessageEdited to group {groupName}");
        await Clients.Group(groupName)
            .MessageEdited(messageResponse);
        Console.WriteLine($"[HUB EditMessage] broadcast done");

        _logger.LogInformation("User {UserId} edited message {MessageId} in conversation {ConversationId}",
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
