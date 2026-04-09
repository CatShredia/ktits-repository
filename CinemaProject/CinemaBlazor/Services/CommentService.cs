using System.Net.Http.Json;
using CinemaBlazor.Models;

namespace CinemaBlazor.Services;

public interface ICommentService
{
    Task<List<Comment>> GetCommentsAsync(int filmId);
    Task<Comment?> CreateCommentAsync(int filmId, CommentCreateDto dto);
}

public class CommentService : ICommentService
{
    private readonly HttpClient _http;

    public CommentService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Comment>> GetCommentsAsync(int filmId)
    {
        var response = await _http.GetAsync($"api/Films/{filmId}/comments");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Comment>>() ?? new List<Comment>();
        }
        return new List<Comment>();
    }

    public async Task<Comment?> CreateCommentAsync(int filmId, CommentCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync($"api/Films/{filmId}/comments", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Comment>();
        }
        return null;
    }
}
