using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TestBlazor3K.ApiRequest.Models;
using TestBlazor3K.Services;

namespace TestBlazor3K.ApiRequest.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;
    private readonly AuthenticationStateProvider _authStateProvider;

    // Ключ для хранения в браузере
    private const string StorageKey = "authToken";

    public UserService(HttpClient httpClient, IJSRuntime js, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _js = js;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Выполняет вход, сохраняет токен и уведомляет приложение об изменении состояния.
    /// </summary>
    public async Task<AuthResponse> LoginAsync(string login, string password)
    {
        var request = new LoginRequest { Login = login, Password = password };

        // Замените 'api/auth/login' на ваш реальный эндпоинт
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                await SetTokenAsync(result.Token);
                
                // Важно: сообщаем Blazor, что пользователь теперь авторизован
                // Это обновит компоненты, использующе <AuthorizeView>
                ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
                
                return result;
            }
        }

        // Обработка ошибок
        var errorContent = await response.Content.ReadAsStringAsync();
        return new AuthResponse 
        { 
            Success = false, 
            Message = $"Login failed: {response.StatusCode}. {errorContent}" 
        };
    }

    /// <summary>
    /// Выполняет выход: удаляет токен и сбрасывает состояние.
    /// </summary>
    public async Task LogoutAsync()
    {
        await RemoveTokenAsync();
        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
    }

    /// <summary>
    /// Проверяет, авторизован ли пользователь прямо сейчас.
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    // --- Методы работы с LocalStorage ---

    private async Task SetTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
        // Также устанавливаем заголовок для текущего клиента
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string?> GetTokenAsync()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", StorageKey);
    }

    private async Task RemoveTokenAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}