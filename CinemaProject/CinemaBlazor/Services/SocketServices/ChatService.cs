
using Microsoft.AspNetCore.SignalR.Client;

namespace CinemaBlazor.ApiRequests
{

    public interface IChatService
    {
        List<string> Users { get; }
        List<MessageModel> Messages { get; }
        Action OnUserListUpdate { get; set; }
        Action OnMessageReceived { get; set; }
        bool IsConnected { get; }

        Task Connect();
        Task Register(string userName);
        Task SendMessage(string fromUser, string toUser, string message);
    }

    public class ChatService : IChatService
    {
        private HubConnection _hubConnection;
        public List<string> Users { get; private set; } = new List<string>();
        public List<MessageModel> Messages { get; private set; } = new List<MessageModel>();
        public Action OnUserListUpdate { get; set; }
        public Action OnMessageReceived { get; set; }

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

            _hubConnection.On<string, string, string, DateTime>("NewMessage", (fromUser, toUser, message, timestamp) =>
            {
                Messages.Add(new MessageModel
                {
                    FromUser = fromUser,
                    ToUser = toUser,
                    Message = message,
                    Time = timestamp
                });
                OnMessageReceived?.Invoke();
            });

            await _hubConnection.StartAsync();
        }
        public async Task Register(string userName)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("Register", userName);
            }
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("SendMessage", fromUser, toUser, message);
            }
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    }

    public class MessageModel
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
