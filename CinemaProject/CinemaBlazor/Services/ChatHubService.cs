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
    public event Action<ConversationCreatedDto>? OnConversationCreated;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public ChatHubService(IConfiguration configuration, ILogger<ChatHubService> logger)
    {
        _logger = logger;
        _serverUrl = configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5268";
    }

    // ! StartAsync - establishes SignalR connection with auth token
    // вызывается из Chat.razor страницы при инициализации
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

    // ! RegisterHandlers - registers SignalR event handlers for receiving messages
    // вызывается внутри StartAsync метода
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

        _hubConnection.On<ConversationCreatedDto>("ConversationCreated", (conversation) =>
        {
            OnConversationCreated?.Invoke(conversation);
        });
    }

    // ! StopAsync - stops SignalR connection
    // вызывается из Chat.razor при уничтожении компонента
    public async Task StopAsync()
    {
        if (!IsConnected || _hubConnection == null)
            return;

        await _hubConnection.StopAsync();
        _logger.LogInformation("SignalR connection stopped");
    }

    // ! JoinConversationAsync - sends join request to SignalR hub
    // вызывается из Chat.razor при входе в чат
    public async Task JoinConversationAsync(int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("joinConversation", conversationId);
    }

    // ! LeaveConversationAsync - sends leave request to SignalR hub
    // вызывается из Chat.razor при выходе из чата
    public async Task LeaveConversationAsync(int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("leaveConversation", conversationId);
    }

    // ! SendMessageAsync - sends message to conversation via SignalR
    // вызывается из Chat.razor при отправке сообщения
    public async Task SendMessageAsync(int conversationId, string content)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("sendMessageToConversation", conversationId, content);
    }

    // ! SendTypingAsync - sends typing notification to conversation via SignalR
    // вызывается из Chat.razor при вводе текста
    public async Task SendTypingAsync(int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("userIsTyping", conversationId);
    }

    // ! DeleteMessageAsync - sends delete message request via SignalR
    // вызывается из Chat.razor при удалении сообщения
    public async Task DeleteMessageAsync(int messageId, int conversationId)
    {
        if (_hubConnection == null) return;
        await _hubConnection.SendAsync("deleteMessage", messageId, conversationId);
    }

    // ! Dispose - disposes SignalR connection (called by DI container)
    // вызывается автоматически при уничтожении сервиса
    public void Dispose()
    {
        if (_hubConnection != null)
        {
            _hubConnection.DisposeAsync().AsTask().Wait();
        }
    }
}
