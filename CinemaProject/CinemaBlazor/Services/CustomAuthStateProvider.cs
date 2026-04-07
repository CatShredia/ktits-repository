using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using CinemaBlazor.Models;

namespace CinemaBlazor.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    // ! GetAuthenticationStateAsync - returns current authentication state (called by framework)
    // вызывается автоматически Blazor AuthenticationStateProvider
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

    // ! SetAuthenticatedUser - saves auth token and notifies auth state change
    // вызывается из AuthService после успешного login/register
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

    // ! SetLogout - clears auth token and notifies auth state change to anonymous
    // вызывается из AuthService.LogoutAsync
    public async Task SetLogout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userRole");
        _http.DefaultRequestHeaders.Authorization = null;

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }

    // ! GetTokenAsync - retrieves JWT token from localStorage
    // вызывается из AuthService и других сервисов
    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    // ! GetToken - synchronous version of GetTokenAsync
    // вызывается синхронно в компонентах
    public string? GetToken()
    {
        return GetTokenAsync().Result;
    }

    // ! ParseClaimsFromJwt - parses JWT token and extracts claims
    // вызывается внутри SetAuthenticatedUser и GetAuthenticationStateAsync
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
        }
        return claims;
    }
}
