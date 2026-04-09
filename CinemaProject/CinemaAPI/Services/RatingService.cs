using CinemaAPI.Data;
using CinemaAPI.Data.Models;
using CinemaAPI.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services;

public interface IRatingService
{
    Task<IEnumerable<RatingResponseDto>> GetAllRatingsAsync();
    Task<RatingResponseDto?> GetRatingByIdAsync(int id);
    Task<Rating> CreateRatingAsync(RatingCreateDto dto, int authorId);
    Task UpdateRatingAsync(int id, RatingUpdateDto dto, int userId, bool isAdmin);
    Task DeleteRatingAsync(int id, int userId, bool isAdmin);
    Task<Rating> GetMyRatingForFilmAsync(int filmId, int userId);
    Task<IEnumerable<RatingResponseDto>> GetMyRatingsAsync(int userId);
}

public class RatingService : IRatingService
{
    private readonly DatabaseContext _context;

    public RatingService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RatingResponseDto>> GetAllRatingsAsync()
    {
        var ratings = await _context.Ratings
            .Include(r => r.Film)
            .Include(r => r.Author)
            .ToListAsync();

        return ratings.Select(MapToRatingDto).ToList();
    }

    public async Task<RatingResponseDto?> GetRatingByIdAsync(int id)
    {
        var rating = await _context.Ratings
            .Include(r => r.Film)
            .Include(r => r.Author)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rating == null)
            return null;

        return MapToRatingDto(rating);
    }

    public async Task<Rating> CreateRatingAsync(RatingCreateDto dto, int authorId)
    {
        var existingRating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.FilmId == dto.FilmId && r.AuthorId == authorId);

        if (existingRating != null)
            throw new InvalidOperationException("You have already rated this film");

        var rating = new Rating
        {
            Value = dto.Value,
            FilmId = dto.FilmId,
            AuthorId = authorId
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return rating;
    }

    public async Task UpdateRatingAsync(int id, RatingUpdateDto dto, int userId, bool isAdmin)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null)
            throw new KeyNotFoundException("Rating not found");

        if (rating.AuthorId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You can only update your own ratings");

        rating.Value = dto.Value;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRatingAsync(int id, int userId, bool isAdmin)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null)
            throw new KeyNotFoundException("Rating not found");

        if (rating.AuthorId != userId && !isAdmin)
            throw new UnauthorizedAccessException("You can only delete your own ratings");

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();
    }

    public async Task<Rating> GetMyRatingForFilmAsync(int filmId, int userId)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.FilmId == filmId && r.AuthorId == userId);

        if (rating == null)
            throw new KeyNotFoundException("No rating found for this film");

        return rating;
    }

    public async Task<IEnumerable<RatingResponseDto>> GetMyRatingsAsync(int userId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.AuthorId == userId)
            .Include(r => r.Film)
            .ToListAsync();

        return ratings.Select(r => new RatingResponseDto
        {
            Id = r.Id,
            Value = r.Value,
            FilmId = r.FilmId,
            FilmName = r.Film?.Name,
            AuthorId = r.AuthorId,
            AuthorName = null
        }).ToList();
    }

    #region Helper Methods

    private RatingResponseDto MapToRatingDto(Rating rating)
    {
        return new RatingResponseDto
        {
            Id = rating.Id,
            Value = rating.Value,
            FilmId = rating.FilmId,
            FilmName = rating.Film?.Name,
            AuthorId = rating.AuthorId,
            AuthorName = rating.Author != null ? $"{rating.Author.Name} {rating.Author.Surname}" : null
        };
    }

    #endregion
}
