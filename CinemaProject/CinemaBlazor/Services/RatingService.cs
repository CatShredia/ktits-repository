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
}

public class RatingService : IRatingService
{
    private readonly HttpClient _http;

    public RatingService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<RatingResponseDto>> GetAllRatingsAsync()
    {
        var response = await _http.GetAsync("api/Ratings");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<RatingResponseDto>>() ?? new List<RatingResponseDto>();
        }
        return new List<RatingResponseDto>();
    }

    public async Task<RatingResponseDto?> GetRatingByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/Ratings/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RatingResponseDto>();
        }
        return null;
    }

    public async Task<Rating?> CreateRatingAsync(RatingCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Ratings", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Rating>();
        }
        return null;
    }

    public async Task<bool> UpdateRatingAsync(int id, RatingUpdateDto dto)
    {
        var response = await _http.PutAsJsonAsync($"api/Ratings/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRatingAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/Ratings/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Rating?> GetMyRatingForFilmAsync(int filmId)
    {
        var response = await _http.GetAsync($"api/Ratings/film/{filmId}/my-rating");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Rating>();
        }
        return null;
    }
}
