using CinemaAPI.Data;
using CinemaAPI.Hubs;
using CinemaAPI.Data.Models.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CinemaAPI.Data.Models;

namespace CinemaAPI.Services;

public interface IChatService
{
    Task<UserChatDto> GetCurrentUserAsync(int userId);
    Task<UserChatDto?> SearchUserByLoginAsync(string login);
    Task<List<ConversationDto>> GetUserConversationsAsync(int userId);
    Task<ConversationDto> CreateConversationAsync(CreateConversationDto dto, int currentUserId);
    Task<List<MessageDto>> GetConversationMessagesAsync(int conversationId, int userId);
    Task<MessageDto> SendMessageAsync(int conversationId, int userId, string content);
    Task DeleteConversationAsync(int conversationId, int userId);
    Task<ConversationDto> AddParticipantsAsync(int conversationId, List<int> userIds, int currentUserId);
    Task RemoveParticipantAsync(int conversationId, int userIdToRemove, int currentUserId);
    Task TransferOwnershipAsync(int conversationId, int newOwnerId, int currentUserId);
    Task ChangeParticipantRoleAsync(int conversationId, int userId, string newRoleName, int currentUserId);
    Task<ConversationDto> GetOrCreateCommentsConversationAsync(int messageId, int userId);
    Task<int> GetCommentsCountAsync(int messageId);
    Task<List<CommentPreviewDto>> GetMessageCommentsPreviewAsync(int messageId);
    int? GetCurrentUserIdFromClaims(ClaimsPrincipal user);
}

public class ChatService : IChatService
{
    private readonly DatabaseContext _context;
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        DatabaseContext context,
        IHubContext<ChatHub, IChatClient> hubContext,
        ILogger<ChatService> logger)
    {
        _context = context;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<UserChatDto> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        return new UserChatDto
        {
            Id = user.Id,
            Login = user.Login?.LoginValue ?? string.Empty,
            Name = user.Name,
            Surname = user.Surname,
            FullName = $"{user.Surname} {user.Name}"
        };
    }

    public async Task<UserChatDto?> SearchUserByLoginAsync(string login)
    {
        var user = await _context.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Login != null && u.Login.LoginValue == login);

        if (user == null)
            return null;

        return new UserChatDto
        {
            Id = user.Id,
            Login = user.Login?.LoginValue ?? string.Empty,
            Name = user.Name,
            Surname = user.Surname,
            FullName = $"{user.Surname} {user.Name}"
        };
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(int userId)
    {
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

        var result = new List<ConversationDto>();
        foreach (var c in conversations)
        {
            var lastMessage = c.Messages.FirstOrDefault();
            var dto = new ConversationDto
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
            result.Add(dto);
        }

        return result;
    }

    public async Task<ConversationDto> CreateConversationAsync(CreateConversationDto dto, int currentUserId)
    {
        var conversationType = await _context.ConversationTypes
            .FirstOrDefaultAsync(ct => ct.Name == dto.ConversationTypeName);

        if (conversationType == null)
            throw new ArgumentException($"Invalid conversation type: {dto.ConversationTypeName}. Valid types: Direct, Group, Channel, Comments");

        if (dto.ParticipantIds == null)
            dto.ParticipantIds = new List<int>();

        if (conversationType.Name == "Direct")
        {
            if (dto.ParticipantIds.Count != 1)
                throw new ArgumentException("Direct chat requires exactly 1 other participant (you + 1 user)");
        }
        else if (conversationType.Name != "Channel")
        {
            if (dto.ParticipantIds.Count == 0)
                throw new ArgumentException("At least one participant is required");
        }

        var allParticipantIds = dto.ParticipantIds.Distinct().ToList();
        if (!allParticipantIds.Contains(currentUserId))
            allParticipantIds.Add(currentUserId);

        if (conversationType.Name == "Direct" && allParticipantIds.Count != 2)
            throw new ArgumentException("Direct chat requires exactly 2 participants (you + 1 user)");

        var existingUsers = await _context.Users
            .Where(u => allParticipantIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        var missingUsers = allParticipantIds.Except(existingUsers).ToList();
        if (missingUsers.Count > 0)
            throw new KeyNotFoundException($"Users with IDs {string.Join(", ", missingUsers)} not found");

        if (conversationType.Name == "Direct")
        {
            var otherUserId = allParticipantIds.First(id => id != currentUserId);
            var existingDirect = await FindExistingDirectChat(currentUserId, otherUserId);
            if (existingDirect != null)
                return await MapConversationToDto(existingDirect);
        }

        var conversation = new Conversation
        {
            ConversationTypeId = conversationType.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        var ownerRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Owner");
        var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");

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
            throw new InvalidOperationException("Failed to create conversation");

        var result = await MapConversationToDto(createdConversation);

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

    public async Task<List<MessageDto>> GetConversationMessagesAsync(int conversationId, int userId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        if (conversation.ConversationType.Name != "Comments")
        {
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (!isParticipant)
                throw new UnauthorizedAccessException("You are not a participant of this conversation");
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

    public async Task<MessageDto> SendMessageAsync(int conversationId, int userId, string content)
    {
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        ConversationParticipant? participant = null;
        if (conversation.ConversationType.Name != "Comments")
        {
            participant = await _context.ConversationParticipants
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

            if (participant == null)
                throw new UnauthorizedAccessException("You are not a participant of this conversation");
        }
        else
        {
            var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");
            participant = new ConversationParticipant { Role = memberRole };
        }

        if (!RolePermissions.CanSendMessage(participant.Role.Name, conversation.ConversationType.Name))
            throw new UnauthorizedAccessException("You don't have permission to send messages in this conversation");

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var createdMessage = await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == message.Id);

        if (createdMessage == null)
            throw new InvalidOperationException("Failed to create message");

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
            userId, messageDto.Id, conversationId);

        return messageDto;
    }

    public async Task DeleteConversationAsync(int conversationId, int userId)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (!isParticipant)
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();
        _context.Messages.RemoveRange(messages);

        var participants = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId)
            .ToListAsync();
        _context.ConversationParticipants.RemoveRange(participants);

        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted conversation {ConversationId}", userId, conversationId);
    }

    public async Task<ConversationDto> AddParticipantsAsync(int conversationId, List<int> userIds, int currentUserId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        if (conversation.ConversationType.Name == "Direct")
            throw new InvalidOperationException("Cannot add participants to a Direct chat");

        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null)
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        if (!RolePermissions.CanAddParticipant(currentParticipant.Role.Name))
            throw new UnauthorizedAccessException("You don't have permission to add participants");

        var existingUsers = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync();

        var missingUsers = userIds.Except(existingUsers).ToList();
        if (missingUsers.Count > 0)
            throw new KeyNotFoundException($"Users with IDs {string.Join(", ", missingUsers)} not found");

        var existingParticipantIds = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversationId)
            .Select(p => p.UserId)
            .ToListAsync();

        var newParticipantIds = userIds.Except(existingParticipantIds).ToList();
        if (newParticipantIds.Count == 0)
            throw new InvalidOperationException("All users are already participants");

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
            throw new InvalidOperationException("Failed to update conversation");

        var result = await MapConversationToDto(updatedConversation);

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

    public async Task RemoveParticipantAsync(int conversationId, int userIdToRemove, int currentUserId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        if (conversation.ConversationType.Name == "Direct")
            throw new InvalidOperationException("Cannot remove participants from a Direct chat");

        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null)
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        var targetParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userIdToRemove);

        if (targetParticipant == null)
            throw new KeyNotFoundException("Participant not found");

        if (userIdToRemove == currentUserId)
            throw new InvalidOperationException("Cannot remove yourself. Delete the entire conversation instead.");

        if (!RolePermissions.CanRemoveParticipant(currentParticipant.Role.Name, targetParticipant.Role.Name))
            throw new UnauthorizedAccessException("You don't have permission to remove this participant");

        _context.ConversationParticipants.Remove(targetParticipant);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {CurrentUserId} removed user {UserId} from conversation {ConversationId}",
            currentUserId, userIdToRemove, conversationId);
    }

    public async Task TransferOwnershipAsync(int conversationId, int newOwnerId, int currentUserId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        if (conversation.ConversationType.Name == "Direct")
            throw new InvalidOperationException("Cannot transfer ownership in a Direct chat");

        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null || currentParticipant.Role.Name != "Owner")
            throw new UnauthorizedAccessException("Only the Owner can transfer ownership");

        var newOwnerParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == newOwnerId);

        if (newOwnerParticipant == null)
            throw new KeyNotFoundException("Target user is not a participant of this conversation");

        var ownerRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Owner");
        var memberRole = await _context.ConversationRoles.FirstAsync(r => r.Name == "Member");

        currentParticipant.RoleId = memberRole.Id;
        newOwnerParticipant.RoleId = ownerRole.Id;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {CurrentUserId} transferred ownership to user {NewOwnerId} in conversation {ConversationId}",
            currentUserId, newOwnerId, conversationId);
    }

    public async Task ChangeParticipantRoleAsync(int conversationId, int userId, string newRoleName, int currentUserId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            throw new KeyNotFoundException("Conversation not found");

        if (conversation.ConversationType.Name == "Direct")
            throw new InvalidOperationException("Cannot change roles in a Direct chat");

        var currentParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUserId);

        if (currentParticipant == null || currentParticipant.Role.Name != "Owner")
            throw new UnauthorizedAccessException("Only the Owner can change roles");

        var targetParticipant = await _context.ConversationParticipants
            .Include(p => p.Role)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (targetParticipant == null)
            throw new KeyNotFoundException("Participant not found");

        if (targetParticipant.Role.Name == "Owner")
            throw new InvalidOperationException("Cannot change the Owner role. Use transfer-ownership instead.");

        var newRole = await _context.ConversationRoles.FirstOrDefaultAsync(r => r.Name == newRoleName);
        if (newRole == null)
            throw new ArgumentException($"Invalid role: {newRoleName}. Valid roles: Admin, Moderator, Member");

        targetParticipant.RoleId = newRole.Id;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {CurrentUserId} changed role of user {UserId} to {NewRole} in conversation {ConversationId}",
            currentUserId, userId, newRoleName, conversationId);
    }

    public async Task<ConversationDto> GetOrCreateCommentsConversationAsync(int messageId, int userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
            throw new KeyNotFoundException("Message not found");

        var channel = await _context.Conversations
            .Include(c => c.ConversationType)
            .FirstOrDefaultAsync(c => c.Id == message.ConversationId && c.ConversationType.Name == "Channel");

        if (channel == null)
            throw new InvalidOperationException("Comments are only available for Channel messages");

        var isChannelParticipant = await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == channel.Id && p.UserId == userId);

        if (!isChannelParticipant)
            throw new UnauthorizedAccessException("You are not a participant of this channel");

        var existingComments = await _context.Conversations
            .Include(c => c.ConversationType)
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                    .ThenInclude(u => u.Login)
            .Include(c => c.Participants)
                .ThenInclude(p => p.Role)
            .FirstOrDefaultAsync(c => c.ParentMessageId == messageId);

        if (existingComments != null)
            return await MapConversationToDto(existingComments);

        var commentsConversation = new Conversation
        {
            ConversationTypeId = 4,
            ParentMessageId = messageId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(commentsConversation);
        await _context.SaveChangesAsync();

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
            throw new InvalidOperationException("Failed to create comments conversation");

        return await MapConversationToDto(created);
    }

    public async Task<int> GetCommentsCountAsync(int messageId)
    {
        var commentsConversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.ParentMessageId == messageId);

        if (commentsConversation == null)
            return 0;

        return await _context.Messages
            .CountAsync(m => m.ConversationId == commentsConversation.Id);
    }

    public async Task<List<CommentPreviewDto>> GetMessageCommentsPreviewAsync(int messageId)
    {
        var commentsConversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.ParentMessageId == messageId);

        if (commentsConversation == null)
            return new List<CommentPreviewDto>();

        var comments = await _context.Messages
            .Where(m => m.ConversationId == commentsConversation.Id)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Take(3)
            .ToListAsync();

        return comments.OrderBy(c => c.CreatedAt).Select(m => new CommentPreviewDto
        {
            Id = m.Id,
            SenderName = $"{m.Sender.Surname} {m.Sender.Name}",
            Content = m.Content,
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public int? GetCurrentUserIdFromClaims(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    #region Helper Methods

    private string GetConversationGroupName(int conversationId)
    {
        return $"conversation_{conversationId}";
    }

    private string GetUserGroupName(int userId)
    {
        return $"user_{userId}";
    }

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

    private async Task<ConversationDto> MapConversationToDto(Conversation conversation)
    {
        var lastMessage = await _context.Messages
            .Where(m => m.ConversationId == conversation.Id)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();

        var participantsWithUsers = await _context.ConversationParticipants
            .Where(p => p.ConversationId == conversation.Id)
            .Include(p => p.User)
                .ThenInclude(u => u.Login)
            .Include(p => p.Role)
            .ToListAsync();

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
