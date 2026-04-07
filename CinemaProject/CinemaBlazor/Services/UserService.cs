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
    Task<bool> CreateLoginAsync(int userId, LoginCreateSimpleDto dto);
    Task<bool> DeleteUserLoginAsync(int userId);
    Task<bool> DeleteUserAsync(int id);
}

public class UserService : IUserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    // ! GetAllUsersAsync - gets all users with search, filter, and sorting
    // вызывается из UsersList.razor страницы
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

    // ! GetUserByIdAsync - gets user by ID with full details
    // вызывается из UserDetails.razor и UserEdit.razor страниц
    public async Task<UserDetailDto?> GetUserByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Users/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDetailDto>();
        }
        return null;
    }

    // ! CreateUserAsync - creates new user with login
    // вызывается из UserCreate.razor страницы
    public async Task<UserDetailDto?> CreateUserAsync(UserCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Users", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDetailDto>();
        }
        return null;
    }

    // ! UpdateUserAsync - updates user by ID
    // вызывается из UserEdit.razor страницы
    public async Task<bool> UpdateUserAsync(int id, UserUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Users/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // ! UpdateUserLoginAsync - updates user's login/password
    // вызывается из UserEdit.razor страницы
    public async Task<bool> UpdateUserLoginAsync(int id, LoginUpdateSimpleDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Users/{id}/login", dto);
        return response.IsSuccessStatusCode;
    }

    // ! CreateLoginAsync - creates login for user (if user has no login)
    // вызывается из UserEdit.razor страницы
    public async Task<bool> CreateLoginAsync(int userId, LoginCreateSimpleDto dto)
    {
        var response = await _http.PostAsJsonAsync($"api/Users/{userId}/login", dto);
        return response.IsSuccessStatusCode;
    }

    // ! DeleteUserLoginAsync - deletes user's login
    // вызывается из UsersList.razor и UserEdit.razor страниц
    public async Task<bool> DeleteUserLoginAsync(int userId)
    {
        var response = await _http.DeleteAsync($"api/Users/{userId}/login");
        return response.IsSuccessStatusCode;
    }

    // ! DeleteUserAsync - deletes user by ID
    // вызывается из UsersList.razor страницы
    public async Task<bool> DeleteUserAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Users/{id}");
        return response.IsSuccessStatusCode;
    }
}
