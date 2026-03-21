using System.Net.Http.Json;
using CinemaBlazor.Models;

namespace CinemaBlazor.Services;

public interface IFilmService
{
    Task<List<Film>> GetAllFilmsAsync(string? sortBy = null, int? genreId = null, string? search = null);
    Task<Film?> GetFilmByIdAsync(int id);
    Task<double> GetAverageRatingAsync(int id);
    Task<Film?> CreateFilmAsync(Film film);
    Task<bool> UpdateFilmAsync(int id, Film film);
    Task<bool> DeleteFilmAsync(int id);
    Task<List<Film>> GetMyFilmsAsync();
}

public class FilmService : IFilmService
{
    private readonly HttpClient _http;

    public FilmService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Film>> GetAllFilmsAsync(string? sortBy = null, int? genreId = null, string? search = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(sortBy))
            queryParams.Add($"sortBy={sortBy}");
        if (genreId.HasValue)
            queryParams.Add($"genreId={genreId.Value}");
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");

        var url = "api/Films";
        if (queryParams.Any())
            url += "?" + string.Join("&", queryParams);

        var response = await _http.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Film>>() ?? new List<Film>();
        }
        
        // Логирование ошибки для отладки
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"FilmService error: {response.StatusCode} - {content}");
        
        return new List<Film>();
    }

    public async Task<Film?> GetFilmByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Films/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Film>();
        }
        return null;
    }

    public async Task<double> GetAverageRatingAsync(int id)
    {
        var response = await _http.GetAsync($"api/Films/{id}/average-rating");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<double>();
        }
        return 0.0;
    }

    public async Task<Film?> CreateFilmAsync(Film film)
    {
        var response = await _http.PostAsJsonAsync("api/Films", film);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Film>();
        }
        return null;
    }

    public async Task<bool> UpdateFilmAsync(int id, Film film)
    {
        var response = await _http.PutAsJsonAsync($"api/Films/{id}", film);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteFilmAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Films/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Film>> GetMyFilmsAsync()
    {
        var response = await _http.GetAsync("api/Films/my-films");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Film>>() ?? new List<Film>();
        }
        return new List<Film>();
    }
}
