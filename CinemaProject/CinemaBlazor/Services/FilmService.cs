using System.Net.Http.Json;
using CinemaBlazor.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace CinemaBlazor.Services;

public interface IFilmService
{
    Task<List<Film>> GetAllFilmsAsync(string? sortBy = null, int? genreId = null, string? search = null);
    Task<Film?> GetFilmByIdAsync(int id);
    Task<double> GetAverageRatingAsync(int id);
    Task<Film?> CreateFilmAsync(Film film, IBrowserFile? imageFile = null, string? externalImageUrl = null);
    Task<bool> UpdateFilmAsync(int id, Film film, IBrowserFile? imageFile = null, string? externalImageUrl = null, bool removeImage = false);
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

    // ! GetAllFilmsAsync - gets all films with optional sorting, genre filter, and search
    // вызывается из FilmsList.razor и Home.razor страниц
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

        var content = await response.Content.ReadAsStringAsync();

        return new List<Film>();
    }

    // ! GetFilmByIdAsync - gets single film by ID
    // вызывается из FilmDetails.razor страницы
    public async Task<Film?> GetFilmByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Films/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Film>();
        }
        return null;
    }

    // ! GetAverageRatingAsync - gets average rating for a film
    // вызывается из FilmDetails.razor страницы
    public async Task<double> GetAverageRatingAsync(int id)
    {
        var response = await _http.GetAsync($"api/Films/{id}/average-rating");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<double>();
        }
        return 0.0;
    }

    // ! CreateFilmAsync - creates new film with optional image upload
    // вызывается из FilmCreate.razor страницы
    public async Task<Film?> CreateFilmAsync(Film film, IBrowserFile? imageFile = null, string? externalImageUrl = null)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(film.Name ?? ""), "dto.Name");
        content.Add(new StringContent(film.Description ?? ""), "dto.Description");
        content.Add(new StringContent(film.ReleaseDate.ToString("yyyy-MM-dd")), "dto.ReleaseDate");
        content.Add(new StringContent(film.GenreId?.ToString() ?? "0"), "dto.GenreId");

        if (!string.IsNullOrEmpty(externalImageUrl))
        {
            content.Add(new StringContent(externalImageUrl), "dto.ExternalImageUrl");
        }

        if (imageFile != null)
        {
            using var stream = imageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileContent = new ByteArrayContent(memoryStream.ToArray());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "imageFile", imageFile.Name);
        }

        var response = await _http.PostAsync("api/Films", content);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Film>();
        }
        return null;
    }

    // ! UpdateFilmAsync - updates film by ID with optional image change
    // вызывается из FilmEdit.razor страницы
    public async Task<bool> UpdateFilmAsync(int id, Film film, IBrowserFile? imageFile = null, string? externalImageUrl = null, bool removeImage = false)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(id.ToString()), "dto.Id");
        content.Add(new StringContent(film.Name ?? ""), "dto.Name");
        content.Add(new StringContent(film.Description ?? ""), "dto.Description");
        content.Add(new StringContent(film.ReleaseDate.ToString("yyyy-MM-dd")), "dto.ReleaseDate");
        content.Add(new StringContent(film.GenreId?.ToString() ?? "0"), "dto.GenreId");

        if (!string.IsNullOrEmpty(externalImageUrl))
        {
            content.Add(new StringContent(externalImageUrl), "dto.ExternalImageUrl");
        }

        content.Add(new StringContent(removeImage.ToString().ToLower()), "dto.RemoveImage");

        if (imageFile != null)
        {
            using var stream = imageFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var fileContent = new ByteArrayContent(memoryStream.ToArray());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
            content.Add(fileContent, "imageFile", imageFile.Name);
        }

        var response = await _http.PutAsync($"api/Films/{id}", content);
        return response.IsSuccessStatusCode;
    }

    // ! DeleteFilmAsync - deletes film by ID
    // вызывается из FilmsList.razor и MyFilms.razor страниц
    public async Task<bool> DeleteFilmAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Films/{id}");
        return response.IsSuccessStatusCode;
    }

    // ! GetMyFilmsAsync - gets films created by current user
    // вызывается из MyFilms.razor страницы
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
