
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
        public int? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
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

    public interface IChatService
    {
        List<string> Users { get; }
        List<ChatMessage> Messages { get; }
        Action OnUserListUpdate { get; set; }
        Action OnMessageReceived { get; set; }
        Action OnHistoryLoaded { get; set; }
        bool IsConnected { get; }
        CurrentUser? CurrentUser { get; }
        UserSearchResult? SelectedContact { get; }

        Task Connect();
        Task Register();
        Task SendMessage(int receiverId, string message);
        Task LoadHistory(int currentUserId, int? contactId);
        Task MarkMessagesAsRead(int currentUserId, int contactId);
        Task Disconnect();
        Task<CurrentUser?> GetCurrentUserAsync();
        Task<List<UserSearchResult>> SearchUsersAsync(string query);
        void SelectContact(UserSearchResult? contact);
    }

    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private HubConnection _hubConnection;
        public List<string> Users { get; private set; } = new List<string>();
        public List<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();
        public Action OnUserListUpdate { get; set; }
        public Action OnMessageReceived { get; set; }
        public Action OnHistoryLoaded { get; set; }
        public CurrentUser? CurrentUser { get; private set; }
        public UserSearchResult? SelectedContact { get; private set; }

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

            _hubConnection.On<string, string, string, DateTime>("NewMessage", async (fromUser, toUser, message, timestamp) =>
            {
                Messages.Add(new ChatMessage
                {
                    SenderName = fromUser,
                    ReceiverName = toUser,
                    Message = message,
                    CreatedAt = timestamp
                });
                OnMessageReceived?.Invoke();

                if (CurrentUser != null && fromUser != $"{CurrentUser.Name} {CurrentUser.Surname}")
                {
                    await MarkMessagesAsRead(CurrentUser.Id, SelectedContact?.Id ?? 0);
                }
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

        public async Task SendMessage(int receiverId, string message)
        {
            Console.WriteLine($"[ChatService] SendMessage called: receiverId={receiverId}, message={message}, State={_hubConnection?.State}");

            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("SendMessage", receiverId, message);
                    Console.WriteLine($"[ChatService] SendMessage succeeded");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ChatService] SendMessage error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[ChatService] SendMessage failed - not connected. State: {_hubConnection?.State}");
            }
        }

        public async Task LoadHistory(int currentUserId, int? contactId)
        {
            try
            {
                var queryParams = $"currentUserId={currentUserId}";
                if (contactId.HasValue)
                {
                    queryParams += $"&contactId={contactId.Value}";
                }

                var response = await _httpClient.GetAsync($"api/chat/history?{queryParams}");

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

        public async Task MarkMessagesAsRead(int currentUserId, int contactId)
        {
            try
            {
                var queryParams = $"currentUserId={currentUserId}&contactId={contactId}";
                await _httpClient.PostAsync($"api/chat/mark-read?{queryParams}", null);
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

        public void SelectContact(UserSearchResult? contact)
        {
            SelectedContact = contact;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    }
}
