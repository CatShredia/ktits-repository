using Microsoft.JSInterop;

namespace CinemaBlazor.Services;

public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _js;

    public LocalStorageService(IJSRuntime js)
    {
        _js = js;
    }

    // ! GetItemAsync - retrieves item from browser localStorage by key
    // вызывается из CustomAuthStateProvider для получения токена
    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
                return default;
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return default;
        }
    }

    // ! SetItemAsync - saves item to browser localStorage
    // вызывается из CustomAuthStateProvider для сохранения токена
    public async Task SetItemAsync<T>(string key, T value)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        await _js.InvokeVoidAsync("localStorage.setItem", key, json);
    }

    // ! RemoveItemAsync - removes item from browser localStorage
    // вызывается из CustomAuthStateProvider при logout
    public async Task RemoveItemAsync(string key)
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
