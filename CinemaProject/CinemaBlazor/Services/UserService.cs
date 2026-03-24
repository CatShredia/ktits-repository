using System.Net.Http.Json;
using CinemaBlazor.Models;

namespace CinemaBlazor.Services;

public interface IUserService
{
    Task<List<UserListDto>> GetAllUsersAsync(
        string? search = null,
        string? searchEmail = null,
        int? roleId = null,
        string? sortBy = null,
        bool sortDescending = false);
    Task<UserDetailDto?> GetUserByIdAsync(int id);
    Task<UserDetailDto?> CreateUserAsync(UserCreateDto dto);
    Task<bool> UpdateUserAsync(int id, UserUpdateDto dto);
    Task<bool> UpdateUserLoginAsync(int id, LoginUpdateSimpleDto dto);
    Task<bool> DeleteUserAsync(int id);
}

public class UserService : IUserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<UserListDto>> GetAllUsersAsync(
        string? search = null,
        string? searchEmail = null,
        int? roleId = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrEmpty(searchEmail))
            queryParams.Add($"searchEmail={Uri.EscapeDataString(searchEmail)}");
        if (roleId.HasValue)
            queryParams.Add($"roleId={roleId.Value}");
        if (!string.IsNullOrEmpty(sortBy))
            queryParams.Add($"sortBy={sortBy}");
        if (sortDescending)
            queryParams.Add("sortDescending=true");

        var url = "api/Users";
        if (queryParams.Any())
            url += "?" + string.Join("&", queryParams);

        var response = await _http.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<UserListDto>>() ?? new List<UserListDto>();
        }
        return new List<UserListDto>();
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Users/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDetailDto>();
        }
        return null;
    }

    public async Task<UserDetailDto?> CreateUserAsync(UserCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Users", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDetailDto>();
        }
        return null;
    }

    public async Task<bool> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Users/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateUserLoginAsync(int id, LoginUpdateSimpleDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Users/{id}/login", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Users/{id}");
        return response.IsSuccessStatusCode;
    }
}
