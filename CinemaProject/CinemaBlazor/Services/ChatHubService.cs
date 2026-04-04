using CinemaBlazor.Models.Chat;
using Microsoft.AspNetCore.SignalR.Client;

namespace CinemaBlazor.Services;

public class ChatHubService : IDisposable
{
    private HubConnection? _hubConnection;
    private readonly ILogger<ChatHubService> _logger;
    private readonly string _serverUrl;

    public event Action<MessageResponse>? OnMessageReceived;
    public event Action<int, int, string>? OnUserTyping;
    public event Action<int, int, string>? OnUserConnected;
    public event Action<int, int, string>? OnUserDisconnected;
    public event Action<int, int>? OnMessageDeleted;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public ChatHubService(IConfiguration configuration, ILogger<ChatHubService> logger)
    {
        _logger = logger;
        _serverUrl = configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5268";
    }

    public async Task StartAsync(string token)
    {
        if (IsConnected)
            return;

        // Для SignalR токен передаётся через query string (WebSockets не поддерживают заголовки)
        if (_hubConnection != null)
        {
            try { await _hubConnection.StopAsync(); } catch { }
            await _hubConnection.DisposeAsync();
        }

        var hubUrl = $"{_serverUrl}/chathub?access_token={Uri.EscapeDataString(token)}";
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();
        await _hubConnection.StartAsync();
        _logger.LogInformation("SignalR connection started");
    }

    private void RegisterHandlers()
    {
        if (_hubConnection == null) return;

        _hubConnection.On<MessageResponse>("ReceiveMessage", (message) =>
        {
            OnMessageReceived?.Invoke(message);
        });

        _hubConnection.On<int, int, string>("UserTyping", (conversationId, userId, userName) =>
        {
            OnUserTyping?.Invoke(conversationId, userId, userName);
        });

        _hubConnection.On<int, int, string>("UserConnected", (conversationId, userId, userName) =>
        {
            OnUserConnected?.Invoke(conversationId, userId, userName);
        });

        _hubConnection.On<int, int, string>("UserDisconnected", (conversationId, userId, userName) =>
        {
            OnUserDisconnected?.Invoke(conversationId, userId, userName);
        });

        _hubConnection.On<int, int>("MessageDeleted", (messageId, conversationId) =>
        {
            OnMessageDeleted?.Invoke(messageId, conversationId);
        });
    }

    public async Task StopAsync()
    {
        if (!IsConnected || _hubConnection == null)
            return;

        await _hubConnection.StopAsync();
        _logger.LogInformation("SignalR connection stopped");
    }

    public async Task JoinConversationAsync(int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("JoinConversation", conversationId);
    }

    public async Task LeaveConversationAsync(int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("LeaveConversation", conversationId);
    }

    public async Task SendMessageAsync(int conversationId, string content)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("SendMessageToConversation", conversationId, content);
    }

    public async Task SendTypingAsync(int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("UserIsTyping", conversationId);
    }

    public async Task DeleteMessageAsync(int messageId, int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("DeleteMessage", messageId, conversationId);
    }

    public void Dispose()
    {
        if (_hubConnection != null)
        {
            _hubConnection.DisposeAsync().AsTask().Wait();
        }
    }
}
