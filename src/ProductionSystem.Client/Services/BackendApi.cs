using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ProductionSystem.Client.Models;

namespace ProductionSystem.Client.Services;

public class BackendApi
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public string? Token { get; private set; }
    public string? Role { get; private set; }
    public string? Login { get; private set; }
    public string? FullName { get; private set; }

    public BackendApi(IConfiguration configuration)
    {
        var baseUrl = configuration["ApiBaseUrl"]?.TrimEnd('/') ?? "http://localhost:5036";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl + "/") };
    }

    public void ApplyAuth(string token, string role, string login, string? fullName)
    {
        Token = token;
        Role = role;
        Login = login;
        FullName = fullName;
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuth()
    {
        Token = null;
        Role = null;
        Login = null;
        FullName = null;
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<(bool Ok, string? Error, AuthResponse? Data)> LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", new { login, password }, _json, ct);
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadFromJsonAsync<AuthResponse>(_json, ct);
            if (body != null)
                ApplyAuth(body.Token, body.Role, body.Login, body.FullName);
            return (true, null, body);
        }

        var msg = await TryReadError(res, ct);
        return (false, msg, null);
    }

    public async Task<(bool Ok, string? Error)> RegisterAsync(string login, string password, string fullName, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/auth/register", new { login, password, fullName }, _json, ct);
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadFromJsonAsync<AuthResponse>(_json, ct);
            if (body != null)
                ApplyAuth(body.Token, body.Role, body.Login, body.FullName);
            return (true, null);
        }

        return (false, await TryReadError(res, ct));
    }

    public async Task<List<WarehouseDto>?> GetWarehousesAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<List<WarehouseDto>>("api/warehouses", _json, ct);
    }

    public async Task<MaterialListResponse?> GetMaterialsAsync(int? warehouseId, CancellationToken ct = default)
    {
        var q = warehouseId is int w ? $"?warehouseId={w}" : "";
        return await _http.GetFromJsonAsync<MaterialListResponse>($"api/materials{q}", _json, ct);
    }

    public async Task<(bool Ok, string? Error)> UpdateMaterialAsync(string article, MaterialUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/materials/{Uri.EscapeDataString(article)}", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteMaterialAsync(string article, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/materials/{Uri.EscapeDataString(article)}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<ComponentListResponse?> GetComponentsAsync(int? warehouseId, CancellationToken ct = default)
    {
        var q = warehouseId is int w ? $"?warehouseId={w}" : "";
        return await _http.GetFromJsonAsync<ComponentListResponse>($"api/components{q}", _json, ct);
    }

    public async Task<(bool Ok, string? Error)> UpdateComponentAsync(string article, ComponentUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/components/{Uri.EscapeDataString(article)}", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteComponentAsync(string article, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/components/{Uri.EscapeDataString(article)}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<WorkerListItemDto>?> GetWorkersAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<WorkerListItemDto>>("api/workers", _json, ct);

    public async Task<WorkerDetailDto?> GetWorkerAsync(int id, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<WorkerDetailDto>($"api/workers/{id}", _json, ct);

    public async Task<(bool Ok, string? Error)> CreateWorkerAsync(WorkerCreateUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/workers", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> UpdateWorkerAsync(int id, WorkerCreateUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/workers/{id}", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteWorkerAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/workers/{id}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<ProductionOperationDto>?> GetOperationsAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<ProductionOperationDto>>("api/production-operations", _json, ct);

    public async Task<(bool Ok, string? Error)> CreateOperationAsync(string name, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/production-operations", new { name }, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    private static async Task<string?> TryReadError(HttpResponseMessage res, CancellationToken ct)
    {
        try
        {
            var text = await res.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(text))
                return res.ReasonPhrase;

            using var doc = JsonDocument.Parse(text);
            if (doc.RootElement.TryGetProperty("message", out var m))
                return m.GetString();
            return text;
        }
        catch
        {
            return res.ReasonPhrase;
        }
    }
}
