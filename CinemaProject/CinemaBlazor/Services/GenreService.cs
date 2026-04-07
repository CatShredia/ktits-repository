using System.Net.Http.Json;
using CinemaBlazor.Models;

namespace CinemaBlazor.Services;

public interface IGenreService
{
    Task<List<Genre>> GetAllGenresAsync();
    Task<Genre?> GetGenreByIdAsync(int id);
    Task<Genre?> CreateGenreAsync(Genre genre);
    Task<bool> UpdateGenreAsync(int id, Genre genre);
    Task<bool> DeleteGenreAsync(int id);
}

public class GenreService : IGenreService
{
    private readonly HttpClient _http;

    public GenreService(HttpClient http)
    {
        _http = http;
    }

    // ! GetAllGenresAsync - gets all genres from API
    // вызывается из GenresList.razor и FilmsList.razor страниц
    public async Task<List<Genre>> GetAllGenresAsync()
    {
        var response = await _http.GetAsync("api/Genres");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Genre>>() ?? new List<Genre>();
        }
        return new List<Genre>();
    }

    // ! GetGenreByIdAsync - gets single genre by ID
    // вызывается из GenreEdit.razor страницы
    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Genres/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Genre>();
        }
        return null;
    }

    // ! CreateGenreAsync - creates new genre
    // вызывается из GenreCreate.razor страницы
    public async Task<Genre?> CreateGenreAsync(Genre genre)
    {
        var response = await _http.PostAsJsonAsync("api/Genres", genre);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Genre>();
        }
        return null;
    }

    // ! UpdateGenreAsync - updates genre by ID
    // вызывается из GenreEdit.razor страницы
    public async Task<bool> UpdateGenreAsync(int id, Genre genre)
    {
        var response = await _http.PutAsJsonAsync($"api/Genres/{id}", genre);
        return response.IsSuccessStatusCode;
    }

    // ! DeleteGenreAsync - deletes genre by ID
    // вызывается из GenresList.razor страницы
    public async Task<bool> DeleteGenreAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Genres/{id}");
        return response.IsSuccessStatusCode;
    }
}
