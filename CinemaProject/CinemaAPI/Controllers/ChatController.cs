using Microsoft.AspNetCore.Mvc;
using CinemaAPI.Data;
using CinemaAPI.Models.Chat;
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

    [HttpGet("history")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistory(
        [FromQuery] int currentUserId,
        [FromQuery] int? contactId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _dbContext.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
            .AsQueryable();

        if (contactId.HasValue)
        {
            query = query.Where(m =>
                (m.SenderId == currentUserId && m.ReceiverId == contactId.Value) ||
                (m.SenderId == contactId.Value && m.ReceiverId == currentUserId));
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = $"{m.Sender.Name} {m.Sender.Surname}",
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver != null ? $"{m.Receiver.Name} {m.Receiver.Surname}" : null,
                Message = m.Message,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            })
            .ToListAsync();

        return messages.OrderBy(m => m.CreatedAt).ToList();
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

    [HttpPost("mark-read")]
    public async Task<ActionResult> MarkMessagesAsRead(
        [FromQuery] int currentUserId,
        [FromQuery] int contactId)
    {
        var messages = await _dbContext.ChatMessages
            .Where(m => m.SenderId == contactId &&
                        m.ReceiverId == currentUserId &&
                        !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}
