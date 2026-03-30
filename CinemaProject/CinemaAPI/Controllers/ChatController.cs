using Microsoft.AspNetCore.Mvc;
using CinemaAPI.Data;
using CinemaAPI.Models.Chat;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("history")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistory(
        [FromQuery] string currentUser,
        [FromQuery] string? contact,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var senderLogin = await _dbContext.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.LoginValue == currentUser);

        if (senderLogin?.User == null)
        {
            return Unauthorized();
        }

        var query = _dbContext.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == senderLogin.User.Id || 
                        (contact != null && m.Receiver != null && m.Receiver.Login!.LoginValue == contact) ||
                        (contact == null && m.ReceiverId == null))
            .AsQueryable();

        if (!string.IsNullOrEmpty(contact))
        {
            var receiverLogin = await _dbContext.Logins
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LoginValue == contact);

            if (receiverLogin?.User != null)
            {
                query = query.Where(m => 
                    (m.SenderId == senderLogin.User.Id && m.ReceiverId == receiverLogin.User.Id) ||
                    (m.SenderId == receiverLogin.User.Id && m.ReceiverId == senderLogin.User.Id));
            }
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
        [FromQuery] string currentUser,
        [FromQuery] string contact)
    {
        var userLogin = await _dbContext.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.LoginValue == currentUser);

        var contactLogin = await _dbContext.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.LoginValue == contact);

        if (userLogin?.User == null || contactLogin?.User == null)
        {
            return BadRequest();
        }

        var messages = await _dbContext.ChatMessages
            .Where(m => m.SenderId == contactLogin.User.Id && 
                        m.ReceiverId == userLogin.User.Id && 
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
