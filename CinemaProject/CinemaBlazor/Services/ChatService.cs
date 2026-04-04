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

    public async Task<UserChatDto?> GetCurrentUserAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync("api/Chat/me");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserChatDto>();
        return null;
    }

    public async Task<UserChatDto?> SearchUserAsync(string login)
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync($"api/Chat/search?login={login}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserChatDto>();
        return null;
    }

    public async Task<List<ConversationDto>> GetConversationsAsync()
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync("api/Chat/conversations");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<ConversationDto>>() ?? new();
        return new();
    }

    public async Task<ConversationDto?> CreateConversationAsync(CreateConversationDto dto)
    {
        await SetAuthHeaderAsync();
        var response = await _http.PostAsJsonAsync("api/Chat/conversations/create", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ConversationDto>();
        return null;
    }

    public async Task<List<MessageDto>> GetMessagesAsync(int conversationId)
    {
        await SetAuthHeaderAsync();
        var response = await _http.GetAsync($"api/Chat/conversations/{conversationId}/messages");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<List<MessageDto>>() ?? new();
        return new();
    }

    public async Task<MessageDto?> SendMessageAsync(int conversationId, string content)
    {
        await SetAuthHeaderAsync();
        var dto = new SendMessageDto { Content = content };
        var response = await _http.PostAsJsonAsync($"api/Chat/conversations/{conversationId}/messages", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<MessageDto>();
        return null;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _authService.GetTokenAsync();
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            _http.DefaultRequestHeaders.Authorization = null;
    }
}
