using Microsoft.AspNetCore.Mvc;
using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.ChatModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public ChatController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var user = await _dbContext.Users
            .Include(u => u.Login)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        return new CurrentUserDto
        {
            Id = user.Id,
            Login = user.Login?.LoginValue ?? "",
            Name = user.Name,
            Surname = user.Surname,
            FullName = $"{user.Name} {user.Surname}"
        };
    }

    [HttpGet("users/search")]
    public async Task<ActionResult<List<UserSearchResultDto>>> SearchUsers([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<UserSearchResultDto>();
        }

        var users = await _dbContext.Logins
            .Include(l => l.User)
            .Where(l => l.LoginValue.Contains(query))
            .Select(l => new UserSearchResultDto
            {
                Id = l.User.Id,
                Login = l.LoginValue,
                Name = l.User.Name,
                Surname = l.User.Surname,
                FullName = $"{l.User.Name} {l.User.Surname}"
            })
            .Take(10)
            .ToListAsync();

        return users;
    }

    [HttpGet("chats")]
    public async Task<ActionResult<List<ChatDto>>> GetUserChats()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
        {
            return Unauthorized();
        }

        var chats = await _dbContext.Chats
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .Where(c => c.IsGeneral || c.User1Id == currentUserId || c.User2Id == currentUserId)
            .ToListAsync();

        var chatDtos = new List<ChatDto>();

        foreach (var chat in chats)
        {
            var lastMessage = chat.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
            var unreadCount = chat.Messages.Count(m => !m.IsRead && m.SenderId != currentUserId);

            var chatDto = new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name,
                IsGeneral = chat.IsGeneral,
                User1Id = chat.User1Id,
                User1Name = chat.User1 != null ? $"{chat.User1.Name} {chat.User1.Surname}" : null,
                User2Id = chat.User2Id,
                User2Name = chat.User2 != null ? $"{chat.User2.Name} {chat.User2.Surname}" : null,
                CreatedAt = chat.CreatedAt,
                UnreadCount = unreadCount,
                LastMessage = lastMessage != null ? new ChatMessageDto
                {
                    Id = lastMessage.Id,
                    SenderId = lastMessage.SenderId,
                    SenderName = $"{lastMessage.Sender.Name} {lastMessage.Sender.Surname}",
                    ChatId = lastMessage.ChatId,
                    Message = lastMessage.Message,
                    CreatedAt = lastMessage.CreatedAt,
                    IsRead = lastMessage.IsRead
                } : null
            };

            chatDtos.Add(chatDto);
        }

        return chatDtos;
    }

    [HttpGet("chats/general")]
    public async Task<ActionResult<ChatDto>> GetGeneralChat()
    {
        var generalChat = await _dbContext.Chats
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.IsGeneral);

        if (generalChat == null)
        {
            return NotFound();
        }

        var lastMessage = generalChat.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

        return new ChatDto
        {
            Id = generalChat.Id,
            Name = generalChat.Name,
            IsGeneral = generalChat.IsGeneral,
            CreatedAt = generalChat.CreatedAt,
            LastMessage = lastMessage != null ? new ChatMessageDto
            {
                Id = lastMessage.Id,
                SenderId = lastMessage.SenderId,
                SenderName = $"{lastMessage.Sender.Name} {lastMessage.Sender.Surname}",
                ChatId = lastMessage.ChatId,
                Message = lastMessage.Message,
                CreatedAt = lastMessage.CreatedAt,
                IsRead = lastMessage.IsRead
            } : null
        };
    }

    [HttpPost("chats/personal/{userId}")]
    public async Task<ActionResult<ChatDto>> CreateOrGetPersonalChat(int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
        {
            return Unauthorized();
        }

        // Проверяем, существует ли уже чат
        var existingChat = await _dbContext.Chats
            .FirstOrDefaultAsync(c =>
                (c.User1Id == currentUserId && c.User2Id == userId) ||
                (c.User1Id == userId && c.User2Id == currentUserId));

        if (existingChat != null)
        {
            return Ok(new ChatDto
            {
                Id = existingChat.Id,
                Name = existingChat.Name,
                IsGeneral = existingChat.IsGeneral,
                User1Id = existingChat.User1Id,
                User1Name = existingChat.User1 != null ? $"{existingChat.User1.Name} {existingChat.User1.Surname}" : null,
                User2Id = existingChat.User2Id,
                User2Name = existingChat.User2 != null ? $"{existingChat.User2.Name} {existingChat.User2.Surname}" : null,
                CreatedAt = existingChat.CreatedAt
            });
        }

        // Создаем новый чат
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var currentUser = await _dbContext.Users.FindAsync(currentUserId);
        var Chat = new Chat
        {
            Name = $"{currentUser?.Name} {currentUser?.Surname} & {user.Name} {user.Surname}",
            User1Id = currentUserId,
            User2Id = userId,
            IsGeneral = false,
            CreatedAt = DateTime.Now
        };

        _dbContext.Chats.Add(Chat);
        await _dbContext.SaveChangesAsync();

        return Ok(new ChatDto
        {
            Id = Chat.Id,
            Name = Chat.Name,
            IsGeneral = Chat.IsGeneral,
            User1Id = Chat.User1Id,
            User1Name = $"{currentUser?.Name} {currentUser?.Surname}",
            User2Id = Chat.User2Id,
            User2Name = $"{user.Name} {user.Surname}",
            CreatedAt = Chat.CreatedAt
        });
    }

    [HttpGet("chats/{chatId}/messages")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetChatMessages(
        int chatId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
        {
            return Unauthorized();
        }

        var chat = await _dbContext.Chats
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            return NotFound();
        }

        // Проверка доступа
        if (!chat.IsGeneral && chat.User1Id != currentUserId && chat.User2Id != currentUserId)
        {
            return Forbid();
        }

        var messages = await _dbContext.ChatMessages
            .Include(m => m.Sender)
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = $"{m.Sender.Name} {m.Sender.Surname}",
                ChatId = m.ChatId,
                Message = m.Message,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            })
            .ToListAsync();

        return messages.OrderBy(m => m.CreatedAt).ToList();
    }

    [HttpPost("chats/{chatId}/messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(int chatId, [FromBody] SendMessageRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
        {
            return Unauthorized();
        }

        if (chatId != request.ChatId)
        {
            return BadRequest("Chat ID mismatch");
        }

        var chat = await _dbContext.Chats.FindAsync(chatId);
        if (chat == null)
        {
            return NotFound();
        }

        // Проверка доступа
        if (!chat.IsGeneral && chat.User1Id != currentUserId && chat.User2Id != currentUserId)
        {
            return Forbid();
        }

        var sender = await _dbContext.Users.FindAsync(currentUserId);
        if (sender == null)
        {
            return NotFound();
        }

        var message = new ChatMessage
        {
            SenderId = currentUserId,
            ChatId = chatId,
            Message = request.Message,
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        _dbContext.ChatMessages.Add(message);
        await _dbContext.SaveChangesAsync();

        return Ok(new ChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = $"{sender.Name} {sender.Surname}",
            ChatId = message.ChatId,
            Message = message.Message,
            CreatedAt = message.CreatedAt,
            IsRead = message.IsRead
        });
    }

    [HttpPost("chats/{chatId}/mark-read")]
    public async Task<ActionResult> MarkMessagesAsRead(int chatId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
        {
            return Unauthorized();
        }

        var messages = await _dbContext.ChatMessages
            .Where(m => m.ChatId == chatId && m.SenderId != currentUserId && !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("online")]
    public async Task<ActionResult<List<OnlineUserDto>>> GetOnlineUsers()
    {
        var users = await _dbContext.Users
            .Select(u => new OnlineUserDto
            {
                UserId = u.Id,
                UserName = $"{u.Name} {u.Surname}",
                IsOnline = true
            })
            .ToListAsync();

        return users;
    }
}
