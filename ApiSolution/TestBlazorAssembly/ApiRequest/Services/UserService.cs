using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using TestBlazorAssembly.ApiRequest.Models;
using TestBlazorAssembly.Services;

namespace TestBlazorAssembly.ApiRequest.Services;

public class UserService
{
    private readonly ApiRequestService _apiRequest;
    private readonly IJSRuntime _js;
    private readonly AuthenticationStateProvider _authStateProvider;

    public UserService(ApiRequestService apiRequest, IJSRuntime js, AuthenticationStateProvider authStateProvider)
    {
        _apiRequest = apiRequest;
        _js = js;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse> LoginAsync(string login, string password)
    {
        var request = new LoginRequest { Login = login, Password = password };

        var response = await _apiRequest.LoginAsync(request);

        if (response != null && response.UserId > 0)
        {
            await SetUserIdAsync(response.UserId);
            await SetRoleIdAsync(response.RoleId);
            await SetUserNameAsync(response.UserName);

            ((CustomAuthStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();

            return response;
        }

        return response ?? new AuthResponse
        {
            Success = false,
            Message = "Login failed."
        };
    }

    public async Task<AuthResponse> RegisterAsync(string login, string password, string userName)
    {
        var result = await _apiRequest.RegisterAsync(login, password, userName);

        if (result.success)
        {
            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful"
            };
        }

        return new AuthResponse
        {
            Success = false,
            Message = result.message ?? "Registration failed."
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
