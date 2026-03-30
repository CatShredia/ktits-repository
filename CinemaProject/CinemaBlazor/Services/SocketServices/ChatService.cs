
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

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

    public interface IChatService
    {
        List<string> Users { get; }
        List<ChatMessage> Messages { get; }
        Action OnUserListUpdate { get; set; }
        Action OnMessageReceived { get; set; }
        Action OnHistoryLoaded { get; set; }
        bool IsConnected { get; }
        string? CurrentUserName { get; }

        Task Connect();
        Task Register(string userName);
        Task SendMessage(string fromUser, string toUser, string message);
        Task LoadHistory(string currentUser, string? contact);
        Task MarkMessagesAsRead(string currentUser, string contact);
        Task Disconnect();
    }

    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private HubConnection _hubConnection;
        public List<string> Users { get; private set; } = new List<string>();
        public List<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();
        public Action OnUserListUpdate { get; set; }
        public Action OnMessageReceived { get; set; }
        public Action OnHistoryLoaded { get; set; }
        public string? CurrentUserName { get; private set; }

        public ChatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Connect()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5268/chat")
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

                if (!string.IsNullOrEmpty(CurrentUserName) && fromUser != CurrentUserName)
                {
                    await MarkMessagesAsRead(CurrentUserName, fromUser);
                }
            });

            await _hubConnection.StartAsync();
        }

        public async Task Register(string userName)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                CurrentUserName = userName;
                await _hubConnection.InvokeAsync("Register", userName);
                await LoadHistory(userName, null);
            }
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("SendMessage", fromUser, toUser, message);
            }
        }

        public async Task LoadHistory(string currentUser, string? contact)
        {
            try
            {
                var queryParams = $"currentUser={Uri.EscapeDataString(currentUser)}";
                if (!string.IsNullOrEmpty(contact))
                {
                    queryParams += $"&contact={Uri.EscapeDataString(contact)}";
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

        public async Task MarkMessagesAsRead(string currentUser, string contact)
        {
            try
            {
                var queryParams = $"currentUser={Uri.EscapeDataString(currentUser)}&contact={Uri.EscapeDataString(contact)}";
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

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    }
}
