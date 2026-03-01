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

    public UserService(HttpClient httpClient, IJSRuntime js, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _js = js;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse> LoginAsync(string login, string password)
    {
        var request = new LoginRequest { Login = login, Password = password };

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (result != null && result.UserId > 0)
            {
                await SetUserIdAsync(result.UserId);
                await SetRoleIdAsync(result.RoleId);
                await SetUserNameAsync(result.UserName);

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

    public async Task<AuthResponse> RegisterAsync(string login, string password, string userName)
    {
        var request = new
        {
            Name = userName,
            Description = string.Empty,
            Login = login,
            Password = password,
            id_Role = 1
        };

        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result != null)
            {
                return result;
            }
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        return new AuthResponse
        {
            Success = false,
            Message = $"Registration failed: {response.StatusCode}. {errorContent}"
        };
    }

    public async Task LogoutAsync()
    {
        await RemoveUserIdAsync();
        await RemoveRoleIdAsync();
        await RemoveUserNameAsync();
        ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var userId = await GetUserIdAsync();
        return userId > 0;
    }

    private async Task SetUserIdAsync(int userId)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "userId", userId.ToString());
    }

    private async Task<int> GetUserIdAsync()
    {
        var userIdStr = await _js.InvokeAsync<string>("localStorage.getItem", "userId");
        return int.TryParse(userIdStr, out var userId) ? userId : 0;
    }

    private async Task RemoveUserIdAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "userId");
    }

    private async Task SetRoleIdAsync(int roleId)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "roleId", roleId.ToString());
    }

    private async Task<int> GetRoleIdAsync()
    {
        var roleIdStr = await _js.InvokeAsync<string>("localStorage.getItem", "roleId");
        return int.TryParse(roleIdStr, out var roleId) ? roleId : 0;
    }

    private async Task RemoveRoleIdAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "roleId");
    }

    private async Task SetUserNameAsync(string userName)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "userName", userName);
    }

    private async Task<string> GetUserNameAsync()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", "userName") ?? string.Empty;
    }

    private async Task RemoveUserNameAsync()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "userName");
    }
}
