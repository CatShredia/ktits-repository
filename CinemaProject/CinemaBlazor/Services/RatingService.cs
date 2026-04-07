using System.Net.Http.Json;
using CinemaBlazor.Models;

namespace CinemaBlazor.Services;

public interface IRatingService
{
    Task<List<RatingResponseDto>> GetAllRatingsAsync();
    Task<RatingResponseDto?> GetRatingByIdAsync(int id);
    Task<Rating?> CreateRatingAsync(RatingCreateDto dto);
    Task<bool> UpdateRatingAsync(int id, RatingUpdateDto dto);
    Task<bool> DeleteRatingAsync(int id);
    Task<Rating?> GetMyRatingForFilmAsync(int filmId);
    Task<List<RatingResponseDto>> GetMyRatingsAsync();
}

public class RatingService : IRatingService
{
    private readonly HttpClient _http;

    public RatingService(HttpClient http)
    {
        _http = http;
    }

    // ! GetAllRatingsAsync - gets all ratings with film and author info
    // вызывается из RatingsList.razor страницы
    public async Task<List<RatingResponseDto>> GetAllRatingsAsync()
    {
        var response = await _http.GetAsync("api/Ratings");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<RatingResponseDto>>() ?? new List<RatingResponseDto>();
        }
        return new List<RatingResponseDto>();
    }

    // ! GetRatingByIdAsync - gets single rating by ID
    // вызывается из RatingsList.razor страницы
    public async Task<RatingResponseDto?> GetRatingByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Ratings/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RatingResponseDto>();
        }
        return null;
    }

    // ! CreateRatingAsync - creates new rating for film
    // вызывается из FilmDetails.razor и RatingsList.razor страниц
    public async Task<Rating?> CreateRatingAsync(RatingCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Ratings", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Rating>();
        }
        return null;
    }

    // ! UpdateRatingAsync - updates rating by ID
    // вызывается из FilmDetails.razor и RatingsList.razor страниц
    public async Task<bool> UpdateRatingAsync(int id, RatingUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Ratings/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // ! DeleteRatingAsync - deletes rating by ID
    // вызывается из FilmDetails.razor и RatingsList.razor страниц
    public async Task<bool> DeleteRatingAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Ratings/{id}");
        return response.IsSuccessStatusCode;
    }

    // ! GetMyRatingForFilmAsync - gets current user's rating for specific film
    // вызывается из FilmDetails.razor страницы
    public async Task<Rating?> GetMyRatingForFilmAsync(int filmId)
    {
        var response = await _http.GetAsync($"api/Ratings/film/{filmId}/my-rating");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Rating>();
        }
        return null;
    }

    // ! GetMyRatingsAsync - gets all ratings created by current user
    // вызывается из RatingsList.razor страницы
    public async Task<List<RatingResponseDto>> GetMyRatingsAsync()
    {
        var response = await _http.GetAsync("api/Ratings/my-ratings");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<RatingResponseDto>>() ?? new List<RatingResponseDto>();
        }
        return new List<RatingResponseDto>();
    }
}
