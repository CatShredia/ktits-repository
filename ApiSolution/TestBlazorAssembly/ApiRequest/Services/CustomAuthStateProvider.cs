using System.Security.Claims;
using System.Text.Json;
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
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Добавляем токен в заголовки HttpClient для будущих запросов
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Парсим токен, чтобы получить имя пользователя и роли (опционально)
        var claims = ParseClaimsFromJwt(token);
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

        return new AuthenticationState(user);
    }

    // Метод для уведомления об изменении состояния (вызывается из AuthService)
    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        // JWT состоит из 3 частей: Header.Payload.Signature
        // Нам нужна вторая часть (Payload)
        var payload = jwt.Split('.')[1];

        // Исправление паддинга для Base64 (иногда без него декодирование падает)
        var padLength = 4 - (payload.Length % 4);
        if (padLength != 4)
        {
            payload += new string('=', padLength);
        }

        // Декодируем Base64 строку в байты
        var bytes = Convert.FromBase64String(payload);

        // ИСПРАВЛЕНИЕ ЗДЕСЬ: используем JsonDocument.Parse с потоком или ReadOnlySpan
        using var document = JsonDocument.Parse(bytes);
        var root = document.RootElement;

        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.Value.EnumerateArray())
                {
                    claims.Add(new Claim(prop.Name, item.ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(prop.Name, prop.Value.ToString()));
            }
        }

        return claims;
    }
}