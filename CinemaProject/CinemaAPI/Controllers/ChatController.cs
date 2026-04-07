using CinemaAPI.Data;
using CinemaAPI.Hubs;
using CinemaAPI.Models;
using CinemaAPI.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers;


// Контроллер для управления чатами и сообщениями

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;
    private readonly ILogger<ChatController> _logger;

    public ChatController(DatabaseContext context,
        IHubContext<ChatHub, IChatClient> hubContext,
        ILogger<ChatController> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _logger = logger;
    }

    // ! GetCurrentUser - returns authenticated user data for chat (UserChatDto)
    // GET /api/Chat/me (из CinemaBlazor через ChatService.GetCurrentUserAsync)
    [HttpGet("me")]
    public async Task<ActionResult<UserChatDto>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        return new UserChatDto
        {
            Id = user.Id,
            Login = user.Login?.LoginValue ?? string.Empty,
            Name = user.Name,
            Surname = user.Surname,
            FullName = $"{user.Surname} {user.Name}"
        };
    }

    // ! SearchUser - finds user by login and returns UserChatDto or 404
    // GET /api/Chat/search?login=... (из CinemaBlazor через ChatService.SearchUserAsync)
    [HttpGet("search")]
    public async Task<ActionResult<UserChatDto>> SearchUser([FromQuery] string login)
    {
        if (string.IsNullOrEmpty(login))
        {
            return BadRequest("Login parameter is required");
        }

        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Login != null && u.Login.LoginValue == login);

        if (user == null)
        {
            return NotFound();
        }

        return new UserChatDto
        {
            Id = user.Id,
            Login = user.Login?.LoginValue ?? string.Empty,
            Name = user.Name,
            Surname = user.Surname,
            FullName = $"{user.Surname} {user.Name}"
        };
    }

    // ! GetUserConversation - returns user's conversations with last messages (List<ConversationDto>)
    // GET /api/Chat/conversations (из CinemaBlazor через ChatService.GetConversationsAsync)
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetUserConversation()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var participantIds = await _context.ConversationParticipants
            .Where(p => p.UserId == userId)
            .Select(p => p.ConversationId)
            .ToListAsync();

        var conversations = await _context.Conversations
            .Where(c => participantIds.Contains(c.Id))
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Messages)
                .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var result = conversations.Select(c =>
        {
            var lastMessage = c.Messages.FirstOrDefault();
            return new ConversationDto
            {
                Id = c.Id,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                Participants = c.Participants.Select(p => new UserChatDto
                {
                    Id = p.User.Id,
                    Login = p.User.Login?.LoginValue ?? string.Empty,
                    Name = p.User.Name,
                    Surname = p.User.Surname,
                    FullName = $"{p.User.Surname} {p.User.Name}"
                }).ToList(),
                LastMessage = lastMessage != null ? new MessageDto
                {
                    Id = lastMessage.Id,
                    ConversationId = lastMessage.ConversationId,
                    SenderId = lastMessage.SenderId,
                    SenderName = $"{lastMessage.Sender.Surname} {lastMessage.Sender.Name}",
                    Content = lastMessage.Content,
                    CreatedAt = lastMessage.CreatedAt,
                    UpdatedAt = lastMessage.UpdatedAt
                } : null
            };
        }).ToList();

        return result;
    }

    // ! CreateOrGetPersonalChat - creates or returns existing chat (Direct/Group) as ConversationDto
    // POST /api/Chat/conversations/create (из CinemaBlazor через ChatService.CreateConversationAsync)
    [HttpPost("conversations/create")]
    public async Task<ActionResult<ConversationDto>> CreateOrGetPersonalChat([FromBody] CreateConversationDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var currentUserId = userId.Value;

        if (dto.ParticipantIds == null || dto.ParticipantIds.Count == 0)
        {
            return BadRequest("ParticipantIds cannot be empty");
        }

        // Проверка существования всех участников
        var allParticipantIds = dto.ParticipantIds.Distinct().ToList();
        if (!allParticipantIds.Contains(currentUserId))
        {
            allParticipantIds.Add(currentUserId);
        }

        var existingUsers = await _context.Users
            .Where(u => allParticipantIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        var missingUsers = allParticipantIds.Except(existingUsers).ToList();
        if (missingUsers.Count > 0)
        {
            return NotFound($"Users with IDs {string.Join(", ", missingUsers)} not found");
        }

        // Минимальное количество участников - 2 (автор + хотя бы 1 пользователь)
        if (allParticipantIds.Count < 2)
        {
            return BadRequest("At least one other participant is required");
        }

        // Если всего 2 участника (автор + 1 пользователь) = Direct чат, иначе Group
        var isDirect = allParticipantIds.Count == 2;
        var conversationType = isDirect ? ConversationType.Direct : ConversationType.Group;

        // Для Direct чата проверяем существующий чат между теми же пользователями
        if (isDirect)
        {
            var otherUserId = allParticipantIds.First(id => id != currentUserId);
            var existingDirect = await FindExistingDirectChat(currentUserId, otherUserId);
            if (existingDirect != null)
            {
                return Ok(await MapConversationToDto(existingDirect));
            }
        }

        // Создание нового чата
        var conversation = new Conversation
        {
            Type = conversationType,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        // Добавление участников
        var participants = allParticipantIds.Select(id => new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = id
        }).ToList();

        _context.ConversationParticipants.AddRange(participants);
        await _context.SaveChangesAsync();

        var createdConversation = await _context.Conversations
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.Id == conversation.Id);

        if (createdConversation == null)
        {
            return NotFound("Failed to create conversation");
        }

        return await MapConversationToDto(createdConversation);
    }

    // ! GetConversationMessages - returns all messages for a conversation sorted by date (List<MessageDto>)
    // GET /api/Chat/conversations/{id}/messages (из CinemaBlazor через ChatService.GetMessagesAsync)
    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetConversationMessages(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        // Проверка, что пользователь является участником чата
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            return Forbid("You are not a participant of this conversation");
        }

        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .Include(m => m.Sender)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            SenderName = $"{m.Sender.Surname} {m.Sender.Name}",
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        }).ToList();
    }

    // ! SendConversationMessage - creates message in chat and sends via SignalR (MessageDto)
    // POST /api/Chat/conversations/{id}/messages (из CinemaBlazor через ChatService.SendMessageAsync)
    [HttpPost("conversations/{conversationId}/messages")]
    public async Task<ActionResult<MessageDto>> SendConversationMessage(int conversationId, [FromBody] SendMessageDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var currentUserId = userId.Value;

        // Проверка существования чата
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Проверка, что пользователь является участником чата
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (!isParticipant)
        {
            return Forbid("You are not a participant of this conversation");
        }

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = currentUserId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var createdMessage = await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == message.Id);

        if (createdMessage == null)
        {
            return NotFound("Failed to create message");
        }

        var messageDto = new MessageDto
        {
            Id = createdMessage.Id,
            ConversationId = createdMessage.ConversationId,
            SenderId = createdMessage.SenderId,
            SenderName = $"{createdMessage.Sender.Surname} {createdMessage.Sender.Name}",
            Content = createdMessage.Content,
            CreatedAt = createdMessage.CreatedAt,
            UpdatedAt = createdMessage.UpdatedAt
        };

        // Отправка сообщения через SignalR всем участникам чата
        await _hubContext.Clients.Group(GetConversationGroupName(conversationId))
            .ReceiveMessage(new MessageResponse
            {
                Id = messageDto.Id,
                ConversationId = messageDto.ConversationId,
                SenderId = messageDto.SenderId,
                SenderName = messageDto.SenderName,
                Content = messageDto.Content,
                CreatedAt = messageDto.CreatedAt,
                UpdatedAt = messageDto.UpdatedAt
            });

        _logger.LogInformation("User {UserId} sent message {MessageId} to conversation {ConversationId}",
            currentUserId, messageDto.Id, conversationId);

        return messageDto;
    }

    #region Helper Methods

    // ! Get userId from JWT token - extracts user ID from claims
    // вызывается внутри всех методов этого контроллера
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    // ! GetConversationGroupName - returns SignalR group name for conversation
    // вызывается внутри методов SendConversationMessage и helper-методов этого контроллера
    private string GetConversationGroupName(int conversationId)
    {
        return $"conversation_{conversationId}";
    }

    // ! FindExistingDirectChat - searches for existing Direct chat between two users
    // вызывается внутри CreateOrGetPersonalChat метода этого контроллера
    private async Task<Conversation?> FindExistingDirectChat(int userId1, int userId2)
    {
        var conversations = await _context.Conversations
            .Include(c => c.Participants)
            .Where(c => c.Type == ConversationType.Direct)
            .ToListAsync();

        foreach (var conversation in conversations)
        {
            var participantIds = conversation.Participants.Select(p => p.UserId).ToList();
            if (participantIds.Contains(userId1) && participantIds.Contains(userId2) && participantIds.Count == 2)
            {
                return conversation;
            }
        }

        return null;
    }

    // ! MapConversationToDto - converts Conversation entity to ConversationDto
    // вызывается внутри CreateOrGetPersonalChat метода этого контроллера
    private async Task<ConversationDto> MapConversationToDto(Conversation conversation)
    {
        var lastMessage = await _context.Messages
            .Where(m => m.ConversationId == conversation.Id)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();

        return new ConversationDto
        {
            Id = conversation.Id,
            Type = conversation.Type,
            CreatedAt = conversation.CreatedAt,
            Participants = conversation.Participants.Select(p => new UserChatDto
            {
                Id = p.User.Id,
                Login = p.User.Login?.LoginValue ?? string.Empty,
                Name = p.User.Name,
                Surname = p.User.Surname,
                FullName = $"{p.User.Surname} {p.User.Name}"
            }).ToList(),
            LastMessage = lastMessage != null ? new MessageDto
            {
                Id = lastMessage.Id,
                ConversationId = lastMessage.ConversationId,
                SenderId = lastMessage.SenderId,
                SenderName = $"{lastMessage.Sender.Surname} {lastMessage.Sender.Name}",
                Content = lastMessage.Content,
                CreatedAt = lastMessage.CreatedAt,
                UpdatedAt = lastMessage.UpdatedAt
            } : null
        };
    }

    #endregion
}
