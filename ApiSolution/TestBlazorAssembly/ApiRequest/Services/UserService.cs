using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TestBlazorAssembly.ApiRequest.Models;
using TestBlazorAssembly.Services;

namespace TestBlazorAssembly.ApiRequest.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _js;
    private readonly AuthenticationStateProvider _authStateProvider;

    // ключ шифрование токена с API
    private const string StorageKey = "l6yQQw5MnLP5";

    public UserService(HttpClient httpClient, IJSRuntime js, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _js = js;
        _authStateProvider = authStateProvider;
    }

    // основной метод входа и регистрации токена
    public async Task<AuthResponse> LoginAsync(string login, string password)
    {
        var request = new LoginRequest { Login = login, Password = password };

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result != null && !string.IsNullOrEmpty(result.Token))
            {
                await SetTokenAsync(result.Token);

                ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();

                return result;
            }
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return new AuthResponse
        {
            Success = false,
            Message = $"Login failed: {response.StatusCode}. {errorContent}"
        };
    }

    public async Task LogoutAsync()
    {
        await RemoveTokenAsync();
        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }


    private async Task SetTokenAsync(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, token);
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