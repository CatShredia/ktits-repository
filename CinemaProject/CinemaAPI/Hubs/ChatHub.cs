using Microsoft.AspNetCore.SignalR;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        private int? GetCurrentUserId()
        {
            // Сначала пробуем из Claims (если авторизация работает через заголовки)
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                Console.WriteLine($"[ChatHub] Found userId from Claims: {userId}");
                return userId;
            }

            // Пробуем получить токен из query string
            var token = Context.GetHttpContext()?.Request.Query["access_token"];
            Console.WriteLine($"[ChatHub] Token from query: {(string.IsNullOrEmpty(token) ? "NULL" : "Present")}");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                try
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var nameIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    if (nameIdClaim != null && int.TryParse(nameIdClaim.Value, out int id))
                    {
                        Console.WriteLine($"[ChatHub] Found userId from JWT: {id}");
                        return id;
                    }
                    else
                    {
                        Console.WriteLine($"[ChatHub] NameIdentifier claim not found in JWT");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ChatHub] Error parsing JWT: {ex.Message}");
                }
            }

            Console.WriteLine($"[ChatHub] GetCurrentUserId returned null");
            return null;
        }

        public async Task Register()
        {
            Console.WriteLine($"[ChatHub] Register called");
            var userId = GetCurrentUserId();
            Console.WriteLine($"[ChatHub] Register: userId = {userId?.ToString() ?? "null"}");

            if (userId.HasValue)
            {
                if (!_userConnections.ContainsKey(userId.Value))
                {
                    _userConnections[userId.Value] = Context.ConnectionId;
                    Console.WriteLine($"[ChatHub] Added connection for userId {userId.Value}: {Context.ConnectionId}");
                }

                // Получаем список онлайн пользователей из памяти, т.к. Dictionary.ContainsKey не транслируется в SQL
                var onlineUserIds = _userConnections.Keys.ToList();
                var onlineUsers = await _dbContext.Users
                    .Where(u => onlineUserIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.Name, u.Surname })
                    .ToListAsync();

                Console.WriteLine($"[ChatHub] Sending UpdateUsers with {onlineUsers.Count} users");
                await Clients.Caller.SendAsync("UpdateUsers", onlineUsers.Select(u => $"{u.Name} {u.Surname}").ToList());
            }
            else
            {
                Console.WriteLine($"[ChatHub] Register failed: userId is null");
                throw new HubException("User not authenticated. Please login first.");
            }
        }

        public async Task SendMessage(int receiverId, string message)
        {
            var senderId = GetCurrentUserId();
            if (senderId.HasValue)
            {
                var sender = await _dbContext.Users.FindAsync(senderId.Value);
                var receiver = await _dbContext.Users.FindAsync(receiverId);

                if (sender != null)
                {
                    var chatMessage = new ChatMessage
                    {
                        SenderId = senderId.Value,
                        ReceiverId = receiverId,
                        Message = message,
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };

                    _dbContext.ChatMessages.Add(chatMessage);
                    await _dbContext.SaveChangesAsync();

                    var senderName = $"{sender.Name} {sender.Surname}";
                    var receiverName = receiver != null ? $"{receiver.Name} {receiver.Surname}" : null;

                    await Clients.All.SendAsync("NewMessage", senderName, receiverName, message, chatMessage.CreatedAt);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionToRemove = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);

            if (connectionToRemove.Key != 0)
            {
                _userConnections.Remove(connectionToRemove.Key);

                // Получаем список онлайн пользователей из памяти
                var onlineUserIds = _userConnections.Keys.ToList();
                var onlineUsers = await _dbContext.Users
                    .Where(u => onlineUserIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.Name, u.Surname })
                    .ToListAsync();

                await Clients.All.SendAsync("UpdateUsers", onlineUsers.Select(u => $"{u.Name} {u.Surname}").ToList());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
