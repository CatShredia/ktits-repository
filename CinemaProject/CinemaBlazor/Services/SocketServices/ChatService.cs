using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using CinemaBlazor.Services;

namespace CinemaBlazor.ApiRequests
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ChatId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsGeneral { get; set; }
        public int? User1Id { get; set; }
        public string? User1Name { get; set; }
        public int? User2Id { get; set; }
        public string? User2Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UnreadCount { get; set; }
        public ChatMessage? LastMessage { get; set; }
    }

    public class OnlineUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class UserSearchResult
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class CurrentUser
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class SendMessageRequest
    {
        public int ChatId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public interface IChatService
    {
        List<string> Users { get; }
        List<ChatMessage> Messages { get; }
        List<Chat> Chats { get; }
        Action OnUserListUpdate { get; set; }
        Action OnMessageReceived { get; set; }
        Action OnHistoryLoaded { get; set; }
        Action OnChatsUpdate { get; set; }
        bool IsConnected { get; }
        CurrentUser? CurrentUser { get; }
        Chat? SelectedChat { get; }
        int? GeneralChatId { get; }

        Task Connect();
        Task Register();
        Task SendMessageToChat(int chatId, string message);
        Task LoadHistory(int chatId);
        Task LoadChats();
        Task MarkMessagesAsRead(int chatId);
        Task Disconnect();
        Task<CurrentUser?> GetCurrentUserAsync();
        Task<List<UserSearchResult>> SearchUsersAsync(string query);
        Task<Chat?> CreateOrGetPersonalChat(int userId);
        Task<Chat?> GetGeneralChat();
        void SelectChat(Chat? chat);
        Task JoinGeneralChat();
    }

    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private HubConnection _hubConnection;
        public List<string> Users { get; private set; } = new List<string>();
        public List<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();
        public List<Chat> Chats { get; private set; } = new List<Chat>();
        public Action OnUserListUpdate { get; set; }
        public Action OnMessageReceived { get; set; }
        public Action OnHistoryLoaded { get; set; }
        public Action OnChatsUpdate { get; set; }
        public CurrentUser? CurrentUser { get; private set; }
        public Chat? SelectedChat { get; private set; }
        public int? GeneralChatId { get; private set; }

        public ChatService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task Connect()
        {
            var token = await _authService.GetTokenAsync();

            Console.WriteLine($"[ChatService] Connecting with token: {(string.IsNullOrEmpty(token) ? "NULL" : token.Substring(0, Math.Min(20, token.Length)))}...");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:5268/chat?access_token={Uri.EscapeDataString(token ?? "")}")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<List<string>>("UpdateUsers", (users) =>
            {
                Users = users;
                OnUserListUpdate?.Invoke();
            });

            _hubConnection.On<int, string, string, DateTime, int>("NewMessage", (chatId, fromUser, message, timestamp, messageId) =>
            {
                var newMessage = new ChatMessage
                {
                    Id = messageId,
                    ChatId = chatId,
                    SenderName = fromUser,
                    Message = message,
                    CreatedAt = timestamp,
                    IsRead = false
                };

                // Если сообщение в выбранном чате или в общем чате, добавляем в список
                if ((SelectedChat != null && SelectedChat.Id == chatId) || 
                    (GeneralChatId.HasValue && GeneralChatId.Value == chatId))
                {
                    Messages.Add(newMessage);
                    OnMessageReceived?.Invoke();
                }

                // Обновляем список чатов
                _ = LoadChats();
            });

            _hubConnection.On<int>("JoinedGeneralChat", (generalChatId) =>
            {
                GeneralChatId = generalChatId;
                Console.WriteLine($"[ChatService] Joined general chat with ID: {generalChatId}");
            });

            _hubConnection.Closed += async (error) =>
            {
                Console.WriteLine($"[ChatService] Connection closed: {error?.Message}");
                await Task.CompletedTask;
            };

            _hubConnection.Reconnecting += async (error) =>
            {
                Console.WriteLine($"[ChatService] Reconnecting: {error?.Message}");
                await Task.CompletedTask;
            };

            _hubConnection.Reconnected += async (connectionId) =>
            {
                Console.WriteLine($"[ChatService] Reconnected with connectionId: {connectionId}");
                await Task.CompletedTask;
            };

            Console.WriteLine($"[ChatService] Starting connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine($"[ChatService] Connection started. State: {_hubConnection.State}");
        }

        public async Task Register()
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("Register");
            }
        }

        public async Task JoinGeneralChat()
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("JoinGeneralChat");
            }
        }

        public async Task SendMessageToChat(int chatId, string message)
        {
            Console.WriteLine($"[ChatService] SendMessageToChat called: chatId={chatId}, message={message}, State={_hubConnection?.State}");

            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("SendMessageToChat", chatId, message);
                    Console.WriteLine($"[ChatService] SendMessageToChat succeeded");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ChatService] SendMessageToChat error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[ChatService] SendMessageToChat failed - not connected. State: {_hubConnection?.State}");
            }
        }

        public async Task LoadHistory(int chatId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/chat/chats/{chatId}/messages");

                if (response.IsSuccessStatusCode)
                {
                    var messages = await response.Content.ReadFromJsonAsync<List<ChatMessage>>();
                    if (messages != null)
                    {
                        Messages = messages;
                        OnHistoryLoaded?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading history: {ex.Message}");
            }
        }

        public async Task LoadChats()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/chat/chats");

                if (response.IsSuccessStatusCode)
                {
                    var chats = await response.Content.ReadFromJsonAsync<List<Chat>>();
                    if (chats != null)
                    {
                        Chats = chats;
                        OnChatsUpdate?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chats: {ex.Message}");
            }
        }

        public async Task<Chat?> GetGeneralChat()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/chat/chats/general");
                if (response.IsSuccessStatusCode)
                {
                    var chat = await response.Content.ReadFromJsonAsync<Chat>();
                    if (chat != null)
                    {
                        GeneralChatId = chat.Id;
                    }
                    return chat;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting general chat: {ex.Message}");
            }
            return null;
        }

        public async Task<Chat?> CreateOrGetPersonalChat(int userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/chat/chats/personal/{userId}", null);
                if (response.IsSuccessStatusCode)
                {
                    var chat = await response.Content.ReadFromJsonAsync<Chat>();
                    return chat;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating personal chat: {ex.Message}");
            }
            return null;
        }

        public async Task MarkMessagesAsRead(int chatId)
        {
            try
            {
                await _httpClient.PostAsync($"api/chat/chats/{chatId}/mark-read", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking messages as read: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("LeaveGeneralChat");
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }

        public async Task<CurrentUser?> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/chat/me");
                if (response.IsSuccessStatusCode)
                {
                    CurrentUser = await response.Content.ReadFromJsonAsync<CurrentUser>();
                    return CurrentUser;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting current user: {ex.Message}");
            }
            return null;
        }

        public async Task<List<UserSearchResult>> SearchUsersAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return new List<UserSearchResult>();
                }

                var response = await _httpClient.GetAsync($"api/chat/users/search?query={Uri.EscapeDataString(query)}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserSearchResult>>();
                    return users ?? new List<UserSearchResult>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching users: {ex.Message}");
            }
            return new List<UserSearchResult>();
        }

        public void SelectChat(Chat? chat)
        {
            SelectedChat = chat;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    }
}
