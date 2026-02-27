using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace TestBlazorAssembly.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(IJSRuntime js, HttpClient httpClient)
    {
        _js = js;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userId = await _js.InvokeAsync<string>("localStorage.getItem", "userId");
        var roleId = await _js.InvokeAsync<string>("localStorage.getItem", "roleId");

        if (string.IsNullOrWhiteSpace(userId))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        if (!string.IsNullOrWhiteSpace(roleId))
        {
            claims.Add(new Claim(ClaimTypes.Role, roleId));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "local"));

        return new AuthenticationState(user);
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
