using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using CinemaBlazor.Models;

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

    // Deprecated sync methods for backward compatibility
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
            // Token might be invalid
        }
        return null;
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
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "role");
            return roleClaim?.Value == role;
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
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = handler.ReadJwtToken(token);
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim != null)
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                return expTime <= DateTimeOffset.Now;
            }
        }
        catch
        {
            return true;
        }
        return true;
    }
}

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public async Task SetAuthenticatedUser(AuthResponseDto authResponse)
    {
        await _localStorage.SetItemAsync("authToken", authResponse.Token);
        await _localStorage.SetItemAsync("userRole", authResponse.Role);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);

        var claims = ParseClaimsFromJwt(authResponse.Token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task SetLogout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userRole");
        _http.DefaultRequestHeaders.Authorization = null;

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    public string? GetToken()
    {
        return GetTokenAsync().Result;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = handler.ReadJwtToken(jwt);
            claims.AddRange(jwtToken.Claims);
        }
        catch
        {
            // Invalid token
        }
        return claims;
    }
}
