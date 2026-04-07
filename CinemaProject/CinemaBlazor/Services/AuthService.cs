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

    // ! LoginAsync - sends login request to API and sets authenticated user state
    // вызывается из Login.razor страницы
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

    // ! RegisterAsync - sends registration request to API and sets authenticated user state
    // вызывается из Register.razor страницы
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

    // ! LogoutAsync - clears auth token and user state, redirects to login
    // вызывается из Logout.razor страницы и NavMenu.razor
    public async Task LogoutAsync()
    {
        await ((CustomAuthStateProvider)_authenticationStateProvider).SetLogout();
    }

    // ! GetCurrentUserAsync - fetches current user profile from API
    // вызывается из Profile.razor и MainLayout.razor
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

    // ! UpdateProfileAsync - updates current user's profile data via API
    // вызывается из ProfileEdit.razor страницы
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

    // ! IsAuthenticatedAsync - checks if user is authenticated and token is valid
    // вызывается из ProtectedRoute.razor и компонентов с защитой маршрутов
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
    }

    // ! IsAuthenticated - synchronous version of IsAuthenticatedAsync (for sync contexts)
    // вызывается синхронно в компонентах, где async недоступен
    public bool IsAuthenticated()
    {
        return IsAuthenticatedAsync().Result;
    }

    // ! IsInRoleAsync - checks if current user has specified role from JWT token
    // вызывается из компонентов с ролевой проверкой (AuthorizeView)
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

    // ! IsInRole - synchronous version of IsInRoleAsync
    // вызывается синхронно в компонентах
    public bool IsInRole(string role)
    {
        try
        {
            return IsInRoleAsync(role).GetAwaiter().GetResult();
        }
        catch
        {
            return false;
        }
    }

    // ! GetTokenAsync - retrieves JWT token from local storage
    // вызывается из всех сервисов для установки auth заголовка
    public async Task<string?> GetTokenAsync()
    {
        return await ((CustomAuthStateProvider)_authenticationStateProvider).GetTokenAsync();
    }

    // ! GetToken - synchronous version of GetTokenAsync
    // вызывается синхронно в компонентах
    public string? GetToken()
    {
        return GetTokenAsync().Result;
    }

    // ! IsTokenExpired - checks if JWT token has expired (currently always returns false - TODO)
    // вызывается внутри IsAuthenticatedAsync метода
    private bool IsTokenExpired(string token)
    {
        // TODO: по истечении определенного времени --> токен не валиден
        return false;
    }
}
