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
            .Include(c => c.ConversationType)
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
            .Include(c => c.Messages)
                .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var result = conversations.Select(c =>
        {
            var lastMessage = c.Messages.FirstOrDefault();
            return new ConversationDto
            {
                Id = c.Id,
                ConversationTypeName = c.ConversationType.Name,
                ParentMessageId = c.ParentMessageId,
                CreatedAt = c.CreatedAt,
                Participants = c.Participants.Select(p => new ParticipantDto
                {
                    UserId = p.User.Id,
                    Login = p.User.Login?.LoginValue ?? string.Empty,
                    Name = p.User.Name,
                    Surname = p.User.Surname,
                    FullName = $"{p.User.Surname} {p.User.Name}",
                    RoleName = p.Role.Name
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

    // ! CreateOrGetPersonalChat - creates or returns existing chat (Direct/Group/Channel/Comments) as ConversationDto
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

        // Сначала ищем тип чата, чтобы понять какие валидации нужны
        var conversationType = await _context.ConversationTypes
            .FirstOrDefaultAsync(ct => ct.Name == dto.ConversationTypeName);

        if (conversationType == null)
        {
            return BadRequest($"Invalid conversation type: {dto.ConversationTypeName}. Valid types: Direct, Group, Channel, Comments");
        }

        // Проверка участников зависит от типа
        if (conversationType.Name == "Channel")
        {
            // Channel: можно создавать без других участников (только владелец)
            if (dto.ParticipantIds == null)
                dto.ParticipantIds = new List<int>();
        }
        else if (conversationType.Name == "Direct")
        {
            // Direct: ровно 1 пользователь (итого 2 с автором)
            if (dto.ParticipantIds == null || dto.ParticipantIds.Count != 1)
            {
                return BadRequest("Direct chat requires exactly 1 other participant (you + 1 user)");
            }
        }
        else
        {
            // Group, Comments: минимум 1 участник (итого 2 с автором)
            if (dto.ParticipantIds == null || dto.ParticipantIds.Count == 0)
            {
                return BadRequest("At least one participant is required");
            }
        }

        // Формируем список всех участников (добавляем автора, если его нет)
        var allParticipantIds = dto.ParticipantIds.Distinct().ToList();
        if (!allParticipantIds.Contains(currentUserId))
        {
            allParticipantIds.Add(currentUserId);
        }

        // Для Channel без других участников — только автор
        // Для Direct — ровно 2 (автор + 1)
        if (conversationType.Name == "Direct" && allParticipantIds.Count != 2)
        {
            return BadRequest("Direct chat requires exactly 2 participants (you + 1 user)");
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

        // Для Direct чата проверяем существующий чат между теми же пользователями
        if (conversationType.Name == "Direct")
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
            ConversationTypeId = conversationType.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        // Получаем роли по умолчанию
        var ownerRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Owner");
        var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");

        // Добавление участников: создатель = Owner, остальные = Member
        var participants = allParticipantIds.Select(id => new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = id,
            RoleId = id == currentUserId ? ownerRole.Id : memberRole.Id
        }).ToList();

        _context.ConversationParticipants.AddRange(participants);
        await _context.SaveChangesAsync();

        var createdConversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Login)
            .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(c => c.Id == conversation.Id);

        if (createdConversation == null)
        {
            return NotFound("Failed to create conversation");
        }

        var result = await MapConversationToDto(createdConversation);

        // Отправляем уведомление всем участникам (кроме создателя) через SignalR
        var createdResponse = new ConversationCreatedResponse
        {
            Id = result.Id,
            ConversationTypeName = result.ConversationTypeName,
            CreatedAt = result.CreatedAt,
            Participants = result.Participants.Select(p => new ConversationParticipantResponse
            {
                UserId = p.UserId,
                FullName = p.FullName,
                Login = p.Login,
                RoleName = p.RoleName
            }).ToList()
        };

        foreach (var participantId in allParticipantIds)
        {
            if (participantId != currentUserId)
            {
                await _hubContext.Clients.Group(GetUserGroupName(participantId))
                    .ConversationCreated(createdResponse);
            }
        }

        return result;
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
        // Для Comments проверка не нужна — доступны всем авторизованным
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        if (conversation.ConversationType.Name != "Comments")
        {
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
            {
                return Forbid();
            }
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
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Для Comments проверка участника не нужна — доступны всем авторизованным
        ConversationParticipant? participant = null;
        if (conversation.ConversationType.Name != "Comments")
        {
            participant = await _context.ConversationParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

            if (participant == null)
            {
                return Forbid();
            }
        }
        else
        {
            // Для Comments создаём "виртуального" участника с ролью Member
            var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");
            participant = new ConversationParticipant { Role = memberRole };
        }

        // Проверка прав на отправку сообщений
        if (!RolePermissions.CanSendMessage(participant.Role.Name, conversation.ConversationType.Name))
        {
            return Forbid("You don't have permission to send messages in this conversation");
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

    // ! DeleteConversation - deletes conversation and its messages (participant only)
    // откуда вызывается: DELETE /api/Chat/conversations/{id} (из CinemaBlazor через ChatService.DeleteConversationAsync)
    [HttpDelete("conversations/{conversationId}")]
    public async Task<IActionResult> DeleteConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Проверка, что пользователь является участником
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
        {
            return Forbid("You are not a participant of this conversation");
        }

        // Удаление сообщений чата
        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();
        _context.Messages.RemoveRange(messages);

        // Удаление участников
        var participants = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId)
            .ToListAsync();
        _context.ConversationParticipants.RemoveRange(participants);

        // Удаление самого чата
        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted conversation {ConversationId}", userId, conversationId);

        return NoContent();
    }

    // ! AddParticipants - adds users to an existing conversation (except Direct)
    // POST /api/Chat/conversations/{id}/participants
    [HttpPost("conversations/{conversationId}/participants")]
    public async Task<ActionResult<ConversationDto>> AddParticipants(int conversationId, [FromBody] List<int> userIds)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Direct чат нельзя изменять
        if (conversation.ConversationType.Name == "Direct")
        {
            return BadRequest("Cannot add participants to a Direct chat");
        }

        // Получаем участника с ролью
        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null)
        {
            return Forbid("You are not a participant of this conversation");
        }

        // Проверка прав на добавление участников
        if (!RolePermissions.CanAddParticipant(currentParticipant.Role.Name))
        {
            return Forbid("You don't have permission to add participants");
        }

        // Проверка существования пользователей
        var existingUsers = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        var missingUsers = userIds.Except(existingUsers).ToList();
        if (missingUsers.Count > 0)
        {
            return NotFound($"Users with IDs {string.Join(", ", missingUsers)} not found");
        }

        // Получаем текущих участников
        var existingParticipantIds = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId)
            .Select(p => p.UserId)
            .ToListAsync();

        // Добавляем только новых (с ролью Member по умолчанию)
        var newParticipantIds = userIds.Except(existingParticipantIds).ToList();
        if (newParticipantIds.Count == 0)
        {
            return BadRequest("All users are already participants");
        }

        var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");

        var newParticipants = newParticipantIds.Select(id => new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = id,
            RoleId = memberRole.Id
        }).ToList();

        _context.ConversationParticipants.AddRange(newParticipants);
        await _context.SaveChangesAsync();

        var updatedConversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Login)
            .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (updatedConversation == null)
        {
            return NotFound("Failed to update conversation");
        }

        var result = await MapConversationToDto(updatedConversation);

        // Уведомляем новых участников через SignalR
        var createdResponse = new ConversationCreatedResponse
        {
            Id = result.Id,
            ConversationTypeName = result.ConversationTypeName,
            CreatedAt = result.CreatedAt,
            Participants = result.Participants.Select(p => new ConversationParticipantResponse
            {
                UserId = p.UserId,
                FullName = p.FullName,
                Login = p.Login,
                RoleName = p.RoleName
            }).ToList()
        };

        foreach (var participantId in newParticipantIds)
        {
            await _hubContext.Clients.Group(GetUserGroupName(participantId))
                .ConversationCreated(createdResponse);
        }

        return result;
    }

    // ! RemoveParticipant - removes a user from a conversation (except Direct)
    // DELETE /api/Chat/conversations/{conversationId}/participants/{userId}
    [HttpDelete("conversations/{conversationId}/participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(int conversationId, int userId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Direct чат нельзя изменять
        if (conversation.ConversationType.Name == "Direct")
        {
            return BadRequest("Cannot remove participants from a Direct chat");
        }

        // Получаем текущего участника с ролью
        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null)
        {
            return Forbid("You are not a participant of this conversation");
        }

        // Получаем целевого участника с ролью
        var targetParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (targetParticipant == null)
        {
            return NotFound("Participant not found");
        }

        // Нельзя удалить самого себя через этот метод
        if (userId == currentUserId)
        {
            return BadRequest("Cannot remove yourself. Delete the entire conversation instead.");
        }

        // Проверка прав на удаление участника
        if (!RolePermissions.CanRemoveParticipant(currentParticipant.Role.Name, targetParticipant.Role.Name))
        {
            return Forbid("You don't have permission to remove this participant");
        }

        _context.ConversationParticipants.Remove(targetParticipant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {CurrentUserId} removed user {UserId} from conversation {ConversationId}",
            currentUserId, userId, conversationId);

        return NoContent();
    }

    // ! TransferOwnership - transfers Owner role to another participant
    // POST /api/Chat/conversations/{id}/transfer-ownership
    [HttpPost("conversations/{conversationId}/transfer-ownership")]
    public async Task<IActionResult> TransferOwnership(int conversationId, [FromBody] int newOwnerId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Transfer только для не-Direct чатов
        if (conversation.ConversationType.Name == "Direct")
        {
            return BadRequest("Cannot transfer ownership in a Direct chat");
        }

        // Получаем текущего участника с ролью
        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null || currentParticipant.Role.Name != "Owner")
        {
            return Forbid("Only the Owner can transfer ownership");
        }

        // Получаем нового владельца
        var newOwnerParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == newOwnerId);

        if (newOwnerParticipant == null)
        {
            return NotFound("Target user is not a participant of this conversation");
        }

        // Находим роли Owner и Member
        var ownerRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Owner");
        var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");

        // Передаём права
        currentParticipant.RoleId = memberRole.Id;
        newOwnerParticipant.RoleId = ownerRole.Id;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {CurrentUserId} transferred ownership to user {NewOwnerId} in conversation {ConversationId}",
            currentUserId, newOwnerId, conversationId);

        return NoContent();
    }

    // ! ChangeRole - changes a participant's role (Owner only)
    // PUT /api/Chat/conversations/{id}/participants/{userId}/role
    [HttpPut("conversations/{conversationId}/participants/{userId}/role")]
    public async Task<IActionResult> ChangeRole(int conversationId, int userId, [FromBody] string newRoleName)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
        {
            return NotFound("Conversation not found");
        }

        // Только для не-Direct чатов
        if (conversation.ConversationType.Name == "Direct")
        {
            return BadRequest("Cannot change roles in a Direct chat");
        }

        // Получаем текущего участника
        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null || currentParticipant.Role.Name != "Owner")
        {
            return Forbid("Only the Owner can change roles");
        }

        // Получаем целевого участника
        var targetParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (targetParticipant == null)
        {
            return NotFound("Participant not found");
        }

        // Нельзя менять роль Owner
        if (targetParticipant.Role.Name == "Owner")
        {
            return BadRequest("Cannot change the Owner role. Use transfer-ownership instead.");
        }

        // Находим новую роль
        var newRole = await _context.ConversationRoles.FirstOrDefaultAsync(r => r.Name == newRoleName);
        if (newRole == null)
        {
            return BadRequest($"Invalid role: {newRoleName}. Valid roles: Admin, Moderator, Member");
        }

        targetParticipant.RoleId = newRole.Id;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {CurrentUserId} changed role of user {UserId} to {NewRole} in conversation {ConversationId}",
            currentUserId, userId, newRoleName, conversationId);

        return NoContent();
    }

    // ! GetOrCreateCommentsConversation - gets or creates a Comments conversation for a Channel message
    // GET /api/Chat/messages/{messageId}/comments
    [HttpGet("messages/{messageId}/comments")]
    public async Task<ActionResult<ConversationDto>> GetOrCreateCommentsConversation(int messageId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        // Находим сообщение
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
        {
            return NotFound("Message not found");
        }

        // Проверяем что сообщение из Channel
        var channel = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == message.ConversationId && c.ConversationType.Name == "Channel");

        if (channel == null)
        {
            return BadRequest("Comments are only available for Channel messages");
        }

        // Проверяем что пользователь — участник Channel
        var isChannelParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == channel.Id && p.UserId == currentUserId);

        if (!isChannelParticipant)
        {
            return Forbid("You are not a participant of this channel");
        }

        // Ищем существующую Comments-конверсацию
        var existingComments = await _context.Conversations
            .Include(c => c.ConversationType)
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Login)
            .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(c => c.ParentMessageId == messageId);

        if (existingComments != null)
        {
            return Ok(await MapConversationToDto(existingComments));
        }

        // Создаём новую Comments-конверсацию с теми же участниками что и Channel
        var commentsConversation = new Conversation
        {
            ConversationTypeId = 4, // Comments
            ParentMessageId = messageId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(commentsConversation);
        await _context.SaveChangesAsync();

        // Копируем участников из Channel
        var channelParticipants = await _context.ConversationParticipants
            .Where(p => p.ConversationId == channel.Id)
            .ToListAsync();

        var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");

        var newParticipants = channelParticipants.Select(p => new ConversationParticipant
        {
            ConversationId = commentsConversation.Id,
            UserId = p.UserId,
            RoleId = memberRole.Id
        }).ToList();

        _context.ConversationParticipants.AddRange(newParticipants);
        await _context.SaveChangesAsync();

        var created = await _context.Conversations
            .Include(c => c.ConversationType)
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Login)
            .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(c => c.Id == commentsConversation.Id);

        if (created == null)
        {
            return NotFound("Failed to create comments conversation");
        }

        return Ok(await MapConversationToDto(created));
    }

    // ! GetCommentsCount - returns comment count for a message
    // GET /api/Chat/messages/{messageId}/comments/count
    [HttpGet("messages/{messageId}/comments/count")]
    public async Task<ActionResult<int>> GetCommentsCount(int messageId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var commentsConversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.ParentMessageId == messageId);

        if (commentsConversation == null)
        {
            return Ok(0);
        }

        var count = await _context.Messages
            .CountAsync(m => m.ConversationId == commentsConversation.Id);

        return Ok(count);
    }

    // ! GetMessageCommentsPreview - returns last 3 comments for a message
    // GET /api/Chat/messages/{messageId}/comments/preview
    [HttpGet("messages/{messageId}/comments/preview")]
    public async Task<ActionResult<List<CommentPreviewDto>>> GetMessageCommentsPreview(int messageId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var commentsConversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.ParentMessageId == messageId);

        if (commentsConversation == null)
        {
            return Ok(new List<CommentPreviewDto>());
        }

        var comments = await _context.Messages
            .Where(m => m.ConversationId == commentsConversation.Id)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Take(3)
            .ToListAsync();

        var preview = comments.OrderBy(c => c.CreatedAt).Select(m => new CommentPreviewDto
        {
            Id = m.Id,
            SenderName = $"{m.Sender.Surname} {m.Sender.Name}",
            Content = m.Content,
            CreatedAt = m.CreatedAt
        }).ToList();

        return Ok(preview);
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

    // ! GetUserGroupName - returns SignalR group name for user ID
    private string GetUserGroupName(int userId)
    {
        return $"user_{userId}";
    }

    // ! FindExistingDirectChat - searches for existing Direct chat between two users
    // вызывается внутри CreateOrGetPersonalChat метода этого контроллера
    private async Task<Conversation?> FindExistingDirectChat(int userId1, int userId2)
    {
        var directType = await _context.ConversationTypes.FirstOrDefaultAsync(ct => ct.Name == "Direct");
        if (directType == null) return null;

        var conversations = await _context.Conversations
            .Include(c => c.Participants)
            .Where(c => c.ConversationTypeId == directType.Id)
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
    // откуда вызывается: вызывается внутри CreateOrGetPersonalChat метода этого контроллера
    private async Task<ConversationDto> MapConversationToDto(Conversation conversation)
    {
        var lastMessage = await _context.Messages
            .Where(m => m.ConversationId == conversation.Id)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();

        // Загружаем участников с Users и Roles явно, чтобы избежать проблем с tracking
        var participantsWithUsers = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversation.Id)
            .Include(p => p.User)
                .ThenInclude(u => u.Login)
            .Include(p => p.Role)
            .ToListAsync();

        // Загружаем тип чата
        var conversationType = await _context.ConversationTypes
            .FirstOrDefaultAsync(ct => ct.Id == conversation.ConversationTypeId);

        return new ConversationDto
        {
            Id = conversation.Id,
            ConversationTypeName = conversationType?.Name ?? string.Empty,
            ParentMessageId = conversation.ParentMessageId,
            CreatedAt = conversation.CreatedAt,
            Participants = participantsWithUsers.Select(p => new ParticipantDto
            {
                UserId = p.User.Id,
                Login = p.User.Login?.LoginValue ?? string.Empty,
                Name = p.User.Name,
                Surname = p.User.Surname,
                FullName = $"{p.User.Surname} {p.User.Name}",
                RoleName = p.Role?.Name ?? "Member"
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
