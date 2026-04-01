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
        private static readonly HashSet<int> _generalChatSubscribers = new HashSet<int>();

        public ChatHub(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                Console.WriteLine($"[ChatHub] Found userId from Claims: {userId}");
                return userId;
            }

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

                var onlineUserIds = _userConnections.Keys.ToList();
                var onlineUsers = await _dbContext.Users
                    .Where(u => onlineUserIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.Name, u.Surname })
                    .ToListAsync();

                Console.WriteLine($"[ChatHub] Sending UpdateUsers with {onlineUsers.Count} users");
                await Clients.Caller.SendAsync("UpdateUsers", onlineUsers.Select(u => $"{u.Name} {u.Surname}").ToList());

                // Подписка на общий чат
                _generalChatSubscribers.Add(userId.Value);
                var generalChat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.IsGeneral);
                if (generalChat != null)
                {
                    await Clients.Caller.SendAsync("JoinedGeneralChat", generalChat.Id);
                }
            }
            else
            {
                Console.WriteLine($"[ChatHub] Register failed: userId is null");
                throw new HubException("User not authenticated. Please login first.");
            }
        }

        public async Task SendMessageToChat(int chatId, string message)
        {
            var senderId = GetCurrentUserId();
            if (!senderId.HasValue)
            {
                throw new HubException("User not authenticated");
            }

            var chat = await _dbContext.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                throw new HubException("Chat not found");
            }

            // Проверка доступа к чату
            if (!chat.IsGeneral && chat.User1Id != senderId.Value && chat.User2Id != senderId.Value)
            {
                throw new HubException("No access to this chat");
            }

            var sender = await _dbContext.Users.FindAsync(senderId.Value);
            if (sender == null)
            {
                throw new HubException("Sender not found");
            }

            var chatMessage = new ChatMessage
            {
                SenderId = senderId.Value,
                ChatId = chatId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _dbContext.ChatMessages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();

            var senderName = $"{sender.Name} {sender.Surname}";

            if (chat.IsGeneral)
            {
                // Отправка всем подписчикам общего чата
                var subscriberConnectionIds = _generalChatSubscribers
                    .Where(uid => _userConnections.ContainsKey(uid))
                    .Select(uid => _userConnections[uid])
                    .ToList();
                
                foreach (var connectionId in subscriberConnectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("NewMessage", chatId, senderName, message, chatMessage.CreatedAt, chatMessage.Id);
                }
            }
            else
            {
                // Отправка участникам личного чата
                var recipientIds = new List<int> { chat.User1Id ?? 0, chat.User2Id ?? 0 }
                    .Where(id => id != 0 && _userConnections.ContainsKey(id))
                    .Select(id => _userConnections[id])
                    .ToList();

                if (recipientIds.Any())
                {
                    await Clients.Clients(recipientIds).SendAsync("NewMessage", chatId, senderName, message, chatMessage.CreatedAt, chatMessage.Id);
                }
            }
        }

        public async Task JoinGeneralChat()
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                _generalChatSubscribers.Add(userId.Value);
                var generalChat = await _dbContext.Chats.FirstOrDefaultAsync(c => c.IsGeneral);
                if (generalChat != null)
                {
                    await Clients.Caller.SendAsync("JoinedGeneralChat", generalChat.Id);
                }
            }
        }

        public async Task LeaveGeneralChat()
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                _generalChatSubscribers.Remove(userId.Value);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionToRemove = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);

            if (connectionToRemove.Key != 0)
            {
                var userId = connectionToRemove.Key;
                _userConnections.Remove(userId);
                _generalChatSubscribers.Remove(userId);

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
