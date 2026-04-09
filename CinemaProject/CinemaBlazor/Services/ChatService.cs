using System.Net.Http.Headers;
using System.Net.Http.Json;
using CinemaBlazor.Models.Chat;

namespace CinemaBlazor.Services;

public interface IChatService
{
    Task<UserChatDto?> GetCurrentUserAsync();
    Task<UserChatDto?> SearchUserAsync(string login);
    Task<List<ConversationDto>> GetConversationsAsync();
    Task<ConversationDto?> CreateConversationAsync(CreateConversationDto dto);
    Task<List<MessageDto>> GetMessagesAsync(int conversationId);
    Task<MessageDto?> SendMessageAsync(int conversationId, string content);
    Task<bool> DeleteConversationAsync(int conversationId);
    Task<ConversationDto?> AddParticipantsAsync(int conversationId, List<int> userIds);
    Task<bool> RemoveParticipantAsync(int conversationId, int userId);
    Task<bool> TransferOwnershipAsync(int conversationId, int newOwnerId);
    Task<bool> ChangeRoleAsync(int conversationId, int userId, string newRoleName);
    Task<string?> GetTokenAsync();
}

public class ChatService : IChatService
{
    private readonly HttpClient _http;
    private readonly IAuthService _authService;

    public ChatService(HttpClient http, IAuthService authService)
    {
        _http = http;
        _authService = authService;
    }

    // ! GetCurrentUserAsync - gets current user data from Chat API
    // вызывается из Chat.razor страницы
    public async Task<UserChatDto?> GetCurrentUserAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync("api/Chat/me");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserChatDto>();
        return null;
    }

    // ! SearchUserAsync - searches for user by login via Chat API
    // вызывается из Chat.razor страницы (поиск собеседника)
    public async Task<UserChatDto?> SearchUserAsync(string login)
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync($"api/Chat/search?login={login}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserChatDto>();
        return null;
    }

    // ! GetConversationsAsync - gets all conversations for current user
    // вызывается из Chat.razor страницы (список чатов)
    public async Task<List<ConversationDto>> GetConversationsAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync("api/Chat/conversations");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<ConversationDto>>() ?? new();
        return new();
    }

    // ! CreateConversationAsync - creates new or returns existing conversation
    // вызывается из Chat.razor страницы (создание чата)
    public async Task<ConversationDto?> CreateConversationAsync(CreateConversationDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/Chat/conversations/create", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ConversationDto>();
        return null;
    }

    // ! GetMessagesAsync - gets all messages for a conversation
    // вызывается из Chat.razor страницы (загрузка сообщений)
    public async Task<List<MessageDto>> GetMessagesAsync(int conversationId)
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync($"api/Chat/conversations/{conversationId}/messages");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<MessageDto>>() ?? new();
        return new();
    }

    // ! SendMessageAsync - sends message to conversation via API
    // откуда вызывается: вызывается из Chat.razor страницы (отправка сообщения)
    public async Task<MessageDto?> SendMessageAsync(int conversationId, string content)
    {
        await SetAuthHeaderAsync();
        var dto = new SendMessageDto { Content = content };
        var response = await _http.PostAsJsonAsync($"api/Chat/conversations/{conversationId}/messages", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<MessageDto>();
        return null;
    }

    // ! DeleteConversationAsync - deletes conversation via API
    // откуда вызывается: вызывается из Chat.razor страницы (удаление чата)
    public async Task<bool> DeleteConversationAsync(int conversationId)
    {
        await SetAuthHeaderAsync();
        var response = await _http.DeleteAsync($"api/Chat/conversations/{conversationId}");
        return response.IsSuccessStatusCode;
    }

    // ! AddParticipantsAsync - adds users to an existing conversation
    public async Task<ConversationDto?> AddParticipantsAsync(int conversationId, List<int> userIds)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/Chat/conversations/{conversationId}/participants", userIds);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ConversationDto>();
        return null;
    }

    // ! RemoveParticipantAsync - removes a user from a conversation
    public async Task<bool> RemoveParticipantAsync(int conversationId, int userId)
    {
        await SetAuthHeaderAsync();
        var response = await _http.DeleteAsync($"api/Chat/conversations/{conversationId}/participants/{userId}");
        return response.IsSuccessStatusCode;
    }

    // ! TransferOwnershipAsync - transfers Owner role to another participant
    public async Task<bool> TransferOwnershipAsync(int conversationId, int newOwnerId)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync($"api/Chat/conversations/{conversationId}/transfer-ownership", newOwnerId);
        return response.IsSuccessStatusCode;
    }

    // ! ChangeRoleAsync - changes a participant's role
    public async Task<bool> ChangeRoleAsync(int conversationId, int userId, string newRoleName)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PutAsJsonAsync($"api/Chat/conversations/{conversationId}/participants/{userId}/role", newRoleName);
        return response.IsSuccessStatusCode;
    }

    // ! GetTokenAsync - gets auth token from AuthService
    // вызывается из ChatHubService для SignalR подключения
    public async Task<string?> GetTokenAsync()
    {
        return await _authService.GetTokenAsync();
    }

    // ! SetAuthHeaderAsync - sets Bearer token in HTTP client headers
    // вызывается внутри всех методов этого сервиса
    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            _http.DefaultRequestHeaders.Authorization = null;
    }
}
