using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using CinemaBlazor.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CinemaBlazor.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(UserLoginDto dto);
    Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto);
    Task LogoutAsync();
    Task<UserResponseDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsInRoleAsync(string role);
    bool IsInRole(string role);
    Task<string?> GetTokenAsync();
    Task<bool> UpdateProfileAsync(UserResponseDto dto);

    bool IsAuthenticated() => IsAuthenticatedAsync().Result;
    string? GetToken() => GetTokenAsync().Result;
}

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(HttpClient http, AuthenticationStateProvider authenticationStateProvider)
    {
        _http = http;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<AuthResponseDto?> LoginAsync(UserLoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", dto);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result != null)
            {
                await ((CustomAuthStateProvider)_authenticationStateProvider).SetAuthenticatedUser(result);
                return result;
            }
        }
        return null;
    }

    public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/register", dto);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result != null)
            {
                await ((CustomAuthStateProvider)_authenticationStateProvider).SetAuthenticatedUser(result);
                return result;
            }
        }
        return null;
    }

    public async Task LogoutAsync()
    {
        await ((CustomAuthStateProvider)_authenticationStateProvider).SetLogout();
    }

    public async Task<UserResponseDto?> GetCurrentUserAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/Auth/me");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserResponseDto>();
            }
        }
        catch
        {
        }
        return null;
    }

    public async Task<bool> UpdateProfileAsync(UserResponseDto dto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync("api/Auth/me", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
    }

    public bool IsAuthenticated()
    {
        return IsAuthenticatedAsync().Result;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == "role" ||
                c.Type == ClaimTypes.Role)?.Value;

            return roleClaim == role;
        }
        catch
        {
            return false;
        }
    }

    public bool IsInRole(string role)
    {
        // Синхронная версия - только для совместимости, используйте IsInRoleAsync
        try
        {
            return IsInRoleAsync(role).GetAwaiter().GetResult();
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        return await ((CustomAuthStateProvider)_authenticationStateProvider).GetTokenAsync();
    }

    public string? GetToken()
    {
        return GetTokenAsync().Result;
    }

    private bool IsTokenExpired(string token)
    {
        // TODO: по истечении определенного времени --> токен не валиден
        return false;
    }
}
