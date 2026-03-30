using Microsoft.AspNetCore.SignalR;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TestSignalR320.Hubs
{
    public class ChatHub : Hub
    {
        private readonly DatabaseContext _dbContext;
        private static readonly Dictionary<int, string> _userConnections = new Dictionary<int, string>();

        public ChatHub(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Register(string userName)
        {
            var login = await _dbContext.Logins
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LoginValue == userName);

            if (login?.User != null)
            {
                var userId = login.User.Id;

                if (!_userConnections.ContainsKey(userId))
                {
                    _userConnections[userId] = Context.ConnectionId;
                }

                var onlineUsers = await _dbContext.Users
                    .Where(u => _userConnections.ContainsKey(u.Id))
                    .Select(u => new { u.Id, u.Name, u.Surname })
                    .ToListAsync();

                await Clients.Caller.SendAsync("UpdateUsers", onlineUsers.Select(u => $"{u.Name} {u.Surname}").ToList());
            }
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            var senderLogin = await _dbContext.Logins
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LoginValue == fromUser);

            var receiverLogin = await _dbContext.Logins
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LoginValue == toUser);

            if (senderLogin?.User != null)
            {
                var chatMessage = new ChatMessage
                {
                    SenderId = senderLogin.User.Id,
                    ReceiverId = receiverLogin?.User.Id,
                    Message = message,
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                _dbContext.ChatMessages.Add(chatMessage);
                await _dbContext.SaveChangesAsync();

                var senderName = $"{senderLogin.User.Name} {senderLogin.User.Surname}";
                var receiverName = receiverLogin != null ? $"{receiverLogin.User.Name} {receiverLogin.User.Surname}" : null;

                await Clients.All.SendAsync("NewMessage", senderName, receiverName, message, chatMessage.CreatedAt);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionToRemove = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);

            if (connectionToRemove.Key != 0)
            {
                _userConnections.Remove(connectionToRemove.Key);

                var onlineUsers = await _dbContext.Users
                    .Where(u => _userConnections.ContainsKey(u.Id))
                    .Select(u => new { u.Id, u.Name, u.Surname })
                    .ToListAsync();

                await Clients.All.SendAsync("UpdateUsers", onlineUsers.Select(u => $"{u.Name} {u.Surname}").ToList());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
