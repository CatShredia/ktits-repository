using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CinemaAPI.Controllers;

// rating CRUD and one
[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public RatingsController(DatabaseContext context)
    {
        _context = context;
    }

    // ! get all rating 
    [HttpGet]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetRatings()
    {
        var ratings = await _context.Ratings
            .Include(r => r.Film)
            .Include(r => r.Author)
            .ToListAsync();

        return ratings.Select(r => new RatingResponseDto
        {
            Id = r.Id,
            Value = r.Value,
            FilmId = r.FilmId,
            FilmName = r.Film?.Name,
            AuthorId = r.AuthorId,
            AuthorName = r.Author != null ? $"{r.Author.Name} {r.Author.Surname}" : null
        }).ToList();
    }

    // ! get rating by userID
    [HttpGet("{id}")]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<RatingResponseDto>> GetRating(int id)
    {
        var rating = await _context.Ratings
            .Include(r => r.Film)
            .Include(r => r.Author)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rating == null)
        {
            return NotFound();
        }

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

    // ! create new rating
    [HttpPost]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<Rating>> PostRating(RatingCreateDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var existingRating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.FilmId == dto.FilmId && r.AuthorId == userId);

        if (existingRating != null)
        {
            return BadRequest("You have already rated this film");
        }

        var rating = new Rating
        {
            Value = dto.Value,
            FilmId = dto.FilmId,
            AuthorId = userId
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating);
    }

    // ! update rating
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutRating(int id, RatingUpdateDto dto)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null)
        {
            return NotFound();
        }

        rating.Value = dto.Value;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ! delete rating
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRating(int id)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null)
        {
            return NotFound();
        }

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ! get ratings by userId 
    [HttpGet("film/{filmId}/my-rating")]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<Rating>> GetMyRatingForFilm(int filmId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.FilmId == filmId && r.AuthorId == userId);

        if (rating == null)
        {
            return NotFound("No rating found for this film");
        }

        return rating;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
}
