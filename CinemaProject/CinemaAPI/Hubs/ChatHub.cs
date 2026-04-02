using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Hubs;


// SignalR хаб для управления чатами в реальном времени
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


    // Подключение пользователя к группе чата
    public async Task JoinConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            await Clients.Caller.UserDisconnected(conversationId, -1, "Unauthorized");
            return;
        }

        // Проверка, что пользователь является участником чата
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            _logger.LogWarning("User {UserId} tried to join conversation {ConversationId} without permission",
                userId, conversationId);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GetConversationGroupName(conversationId));

        // Уведомление других участников о подключении
        var user = await _context.Users.FindAsync(userId.Value);
        if (user != null)
        {
            await Clients.GroupExcept(GetConversationGroupName(conversationId), Context.ConnectionId)
                .UserConnected(conversationId, userId.Value, $"{user.Surname} {user.Name}");
        }

        _logger.LogInformation("User {UserId} joined conversation {ConversationId}", userId.Value, conversationId);
    }


    // Отключение пользователя от группы чата
    public async Task LeaveConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetConversationGroupName(conversationId));

        // Уведомление других участников об отключении
        var user = await _context.Users.FindAsync(userId.Value);
        if (user != null)
        {
            await Clients.Group(GetConversationGroupName(conversationId))
                .UserDisconnected(conversationId, userId.Value, $"{user.Surname} {user.Name}");
        }

        _logger.LogInformation("User {UserId} left conversation {ConversationId}", userId.Value, conversationId);
    }


    // Отправка сообщения в чат
    public async Task SendMessageToConversation(int conversationId, string content)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        // Проверка, что пользователь является участником чата
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            _logger.LogWarning("User {UserId} tried to send message to conversation {ConversationId} without permission",
                userId, conversationId);
            return;
        }

        // Валидация контента
        if (string.IsNullOrWhiteSpace(content) || content.Length > 1000)
        {
            return;
        }

        // Создание сообщения
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = userId.Value,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Получение данных отправителя
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

        // Отправка сообщения всем участникам чата
        await Clients.Group(GetConversationGroupName(conversationId))
            .ReceiveMessage(messageResponse);

        _logger.LogInformation("User {UserId} sent message to conversation {ConversationId}", userId, conversationId);
    }


    // Уведомление о том, что пользователь печатает
    public async Task UserIsTyping(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return;
        }

        // Проверка, что пользователь является участником чата
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


    // Удаление сообщения (только автор может удалить своё сообщение)
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

        // Проверка, что пользователь является автором сообщения
        if (message.SenderId != userId.Value)
        {
            _logger.LogWarning("User {UserId} tried to delete message {MessageId} without permission",
                userId.Value, messageId);
            return;
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        // Уведомление всех участников об удалении
        await Clients.Group(GetConversationGroupName(conversationId))
            .MessageDeleted(messageId, conversationId);

        _logger.LogInformation("User {UserId} deleted message {MessageId} from conversation {ConversationId}",
            userId.Value, messageId, conversationId);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            // Добавляем подключение к группе пользователя для личных уведомлений
            await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId.Value));
            _logger.LogInformation("User {UserId} connected. ConnectionId: {ConnectionId}",
                userId.Value, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

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

    #region Helper Methods

    private int? GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    private string GetConversationGroupName(int conversationId)
    {
        return $"conversation_{conversationId}";
    }

    private string GetUserGroupName(int userId)
    {
        return $"user_{userId}";
    }

    #endregion
}
