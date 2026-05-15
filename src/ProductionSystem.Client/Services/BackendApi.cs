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
        var inner = new HttpClientHandler();
        var logging = new ApiRequestLoggingHandler { InnerHandler = inner };
        _http = new HttpClient(logging) { BaseAddress = new Uri(baseUrl + "/") };
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
            var body = await ReadAuthResponseAsync(res, ct);
            if (body is null)
                return (false, "Некорректный ответ сервера.", null);
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
            if (await ReadAuthResponseAsync(res, ct) is null)
                return (false, "Некорректный ответ сервера.");
            return (true, null);
        }

        return (false, await TryReadError(res, ct));
    }

    public async Task<List<WarehouseDto>?> GetWarehousesAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<WarehouseDto>>("api/warehouses", ct);

    public async Task<MaterialListResponse?> GetMaterialsAsync(int? warehouseId, CancellationToken ct = default)
    {
        var q = warehouseId is int w ? $"?warehouseId={w}" : "";
        return await GetJsonAsync<MaterialListResponse>($"api/materials{q}", ct);
    }

    public async Task<(bool Ok, string? Error)> UpdateMaterialAsync(int id, MaterialUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/materials/{id}", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteMaterialAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/materials/{id}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<ComponentListResponse?> GetComponentsAsync(int? warehouseId, CancellationToken ct = default)
    {
        var q = warehouseId is int w ? $"?warehouseId={w}" : "";
        return await GetJsonAsync<ComponentListResponse>($"api/components{q}", ct);
    }

    public async Task<(bool Ok, string? Error)> UpdateComponentAsync(int id, ComponentUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/components/{id}", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteComponentAsync(int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/components/{id}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<WorkerListItemDto>?> GetWorkersAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<WorkerListItemDto>>("api/workers", ct);

    public async Task<WorkerDetailDto?> GetWorkerAsync(int id, CancellationToken ct = default) =>
        await GetJsonAsync<WorkerDetailDto>($"api/workers/{id}", ct);

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
        await GetJsonAsync<List<ProductionOperationDto>>("api/production-operations", ct);

    public async Task<(bool Ok, string? Error)> CreateOperationAsync(string name, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/production-operations", new { name }, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<OrderListItemDto>?> GetOrdersAsync(string? filter = null, CancellationToken ct = default)
    {
        var q = string.IsNullOrWhiteSpace(filter) ? "" : $"?filter={Uri.EscapeDataString(filter)}";
        return await GetJsonAsync<List<OrderListItemDto>>($"api/orders{q}", ct);
    }

    public async Task<OrderDetailDto?> GetOrderAsync(string number, CancellationToken ct = default) =>
        await GetJsonAsync<OrderDetailDto>($"api/orders/{Uri.EscapeDataString(number)}", ct);

    public async Task<List<OrderStatusHistoryDto>?> GetOrderHistoryAsync(string number, CancellationToken ct = default) =>
        await GetJsonAsync<List<OrderStatusHistoryDto>>($"api/orders/{Uri.EscapeDataString(number)}/history", ct);

    public async Task<List<CustomerUserDto>?> GetCustomersAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<CustomerUserDto>>("api/orders/customers", ct);

    public async Task<(bool Ok, string? Error, OrderDetailDto? Data)> CreateOrderAsync(
        OrderCreateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/orders", body, _json, ct);
        if (res.IsSuccessStatusCode)
        {
            var data = await res.Content.ReadFromJsonAsync<OrderDetailDto>(_json, ct);
            return (true, null, data);
        }

        return (false, await TryReadError(res, ct), null);
    }

    public async Task<(bool Ok, string? Error)> UpdateOrderAsync(
        string number, OrderCreateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/orders/{Uri.EscapeDataString(number)}", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteOrderAsync(string number, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/orders/{Uri.EscapeDataString(number)}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> ChangeOrderStatusAsync(
        string number, OrderStatusChangeRequest body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"api/orders/{Uri.EscapeDataString(number)}/status", body, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> CancelOrderByCustomerAsync(
        string number, string? comment = null, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(
            $"api/orders/{Uri.EscapeDataString(number)}/cancel",
            new OrderStatusChangeRequest { Comment = comment },
            _json,
            ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<WorkshopDto>?> GetWorkshopsAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<WorkshopDto>>("api/workshops", ct);

    public async Task<(bool Ok, string? Error)> SaveWorkshopLayoutAsync(
        int workshopId, List<WorkshopLayoutItemDto> items, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/workshops/{workshopId}/layout", new { items }, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<EquipmentFailureDto>?> GetEquipmentFailuresAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<EquipmentFailureDto>>("api/equipment-failures", ct);

    public async Task<(bool Ok, string? Error)> CreateEquipmentFailureAsync(
        string marking, DateTime startedAt, string reason, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/equipment-failures",
            new { equipmentMarking = marking, startedAt, reason }, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> EndEquipmentFailureAsync(int id, DateTime endedAt, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"api/equipment-failures/{id}/end", new { endedAt }, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<List<EquipmentListItemDto>?> GetEquipmentAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<EquipmentListItemDto>>("api/equipment", ct);

    public async Task<List<QualityCheckDto>?> GetQualityChecksAsync(string orderNumber, CancellationToken ct = default) =>
        await GetJsonAsync<List<QualityCheckDto>>(
            $"api/orders/{Uri.EscapeDataString(orderNumber)}/quality-checks", ct);

    public async Task<(bool Ok, string? Error)> UpsertQualityCheckAsync(
        string orderNumber, string parameterName, string grade, string? comment, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(
            $"api/orders/{Uri.EscapeDataString(orderNumber)}/quality-checks",
            new { parameterName, grade, comment },
            _json,
            ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    private async Task<AuthResponse?> ReadAuthResponseAsync(HttpResponseMessage res, CancellationToken ct)
    {
        var body = await res.Content.ReadFromJsonAsync<AuthResponse>(_json, ct);
        if (body is null || string.IsNullOrWhiteSpace(body.Token))
            return null;

        ApplyAuth(body.Token, body.Role, body.Login, body.FullName);
        return body;
    }

    public async Task<List<ProductListItemDto>?> GetProductsAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<ProductListItemDto>>("api/products", ct);

    public async Task<ProductDetailDto?> GetProductAsync(string name, CancellationToken ct = default) =>
        await GetJsonAsync<ProductDetailDto>($"api/products/{Uri.EscapeDataString(name)}", ct);

    public async Task<(bool Ok, string? Error, ProductDetailDto? Data)> UpdateProductAsync(
        string name, ProductUpdateRequest body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync($"api/products/{Uri.EscapeDataString(name)}", body, _json, ct);
        if (res.IsSuccessStatusCode)
        {
            var data = await res.Content.ReadFromJsonAsync<ProductDetailDto>(_json, ct);
            return (true, null, data);
        }

        return (false, await TryReadError(res, ct), null);
    }

    public async Task<(bool Ok, string? Error)> AddProductDrawingAsync(
        string name, string title, string source, string? contentBase64, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync($"api/products/{Uri.EscapeDataString(name)}/drawings",
            new { title, source, contentBase64 }, _json, ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<(bool Ok, string? Error)> DeleteProductDrawingAsync(
        string name, int id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/products/{Uri.EscapeDataString(name)}/drawings/{id}", ct);
        return res.IsSuccessStatusCode ? (true, null) : (false, await TryReadError(res, ct));
    }

    public async Task<OperationsCatalogDto?> GetOperationsCatalogAsync(CancellationToken ct = default) =>
        await GetJsonAsync<OperationsCatalogDto>("api/products/operations-catalog", ct);

    public async Task<List<ProductMaterialLineDto>?> GetMaterialsCatalogAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<ProductMaterialLineDto>>("api/products/materials-catalog", ct);

    public async Task<List<ProductComponentLineDto>?> GetComponentsCatalogAsync(CancellationToken ct = default) =>
        await GetJsonAsync<List<ProductComponentLineDto>>("api/products/components-catalog", ct);

    public async Task<OrderPlanningDto?> GetOrderPlanningAsync(string orderNumber, CancellationToken ct = default) =>
        await GetJsonAsync<OrderPlanningDto>($"api/orders/{Uri.EscapeDataString(orderNumber)}/planning", ct);

    public async Task<InventoryReportResponse?> GetInventoryReportAsync(
        string kind, string? type, CancellationToken ct = default)
    {
        var q = $"?kind={Uri.EscapeDataString(kind)}";
        if (!string.IsNullOrWhiteSpace(type))
            q += $"&type={Uri.EscapeDataString(type)}";
        return await GetJsonAsync<InventoryReportResponse>($"api/reports/inventory{q}", ct);
    }

    public async Task<List<string>?> GetInventoryReportTypesAsync(string kind, CancellationToken ct = default) =>
        await GetJsonAsync<List<string>>($"api/reports/inventory/types?kind={Uri.EscapeDataString(kind)}", ct);

    private async Task<T?> GetJsonAsync<T>(string url, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(Token))
            return default;

        var res = await _http.GetAsync(url, ct);
        if (!res.IsSuccessStatusCode)
            return default;

        return await res.Content.ReadFromJsonAsync<T>(_json, ct);
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
