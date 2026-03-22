using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public FilmsController(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all films with optional sorting and filtering
    /// </summary>
    /// <param name="sortBy">Sort by: name, releaseDate, rating</param>
    /// <param name="genreId">Filter by genre ID</param>
    /// <param name="search">Search by name</param>
    /// <returns>List of films</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FilmDto>>> GetFilms(
        [FromQuery] string? sortBy = null,
        [FromQuery] int? genreId = null,
        [FromQuery] string? search = null)
    {
        var query = _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .AsQueryable();

        // Filtering
        if (genreId.HasValue)
        {
            query = query.Where(f => f.GenreId == genreId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(f => f.Name.Contains(search));
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "name" => query.OrderBy(f => f.Name),
            "name_desc" => query.OrderByDescending(f => f.Name),
            "releasedate" => query.OrderBy(f => f.ReleaseDate),
            "releasedate_desc" => query.OrderByDescending(f => f.ReleaseDate),
            "rating" => query.OrderBy(f => f.Ratings!.Average(r => r.Value)),
            "rating_desc" => query.OrderByDescending(f => f.Ratings!.Average(r => r.Value)),
            _ => query
        };

        var films = await query.ToListAsync();
        
        return films.Select(f => new FilmDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            ReleaseDate = f.ReleaseDate,
            GenreId = f.GenreId,
            Genre = f.Genre != null ? new GenreDto
            {
                Id = f.Genre.Id,
                Name = f.Genre.Name,
                Description = f.Genre.Description
            } : null,
            ImageUrl = f.ImageUrl,
            AuthorId = f.AuthorId,
            Author = f.Author != null ? new UserBriefDto
            {
                Id = f.Author.Id,
                Name = f.Author.Name,
                Surname = f.Author.Surname
            } : null,
            AverageRating = f.Ratings?.Any() == true ? f.Ratings.Average(r => r.Value) : 0,
            RatingsCount = f.Ratings?.Count ?? 0
        }).ToList();
    }

    /// <summary>
    /// Get a specific film by ID
    /// </summary>
    /// <param name="id">Film ID</param>
    /// <returns>The film</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<FilmDto>> GetFilm(int id)
    {
        var film = await _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (film == null)
        {
            return NotFound();
        }

        return new FilmDto
        {
            Id = film.Id,
            Name = film.Name,
            Description = film.Description,
            ReleaseDate = film.ReleaseDate,
            GenreId = film.GenreId,
            Genre = film.Genre != null ? new GenreDto
            {
                Id = film.Genre.Id,
                Name = film.Genre.Name,
                Description = film.Genre.Description
            } : null,
            ImageUrl = film.ImageUrl,
            AuthorId = film.AuthorId,
            Author = film.Author != null ? new UserBriefDto
            {
                Id = film.Author.Id,
                Name = film.Author.Name,
                Surname = film.Author.Surname
            } : null,
            AverageRating = film.Ratings?.Any() == true ? film.Ratings.Average(r => r.Value) : 0,
            RatingsCount = film.Ratings?.Count ?? 0
        };
    }

    /// <summary>
    /// Get average rating for a specific film
    /// </summary>
    /// <param name="id">Film ID</param>
    /// <returns>Average rating value</returns>
    [HttpGet("{id}/average-rating")]
    [AllowAnonymous]
    public async Task<ActionResult<double>> GetAverageRating(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        var ratings = await _context.Ratings
            .Where(r => r.FilmId == id)
            .ToListAsync();

        if (ratings.Count == 0)
        {
            return Ok(0.0);
        }

        return ratings.Average(r => r.Value);
    }

    /// <summary>
    /// Create a new film (Admin only)
    /// </summary>
    /// <param name="film">Film data</param>
    /// <returns>Created film</returns>
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<FilmDto>> PostFilm(Film film)
    {
        var userId = GetCurrentUserId();
        if (userId != null)
        {
            film.AuthorId = userId;
        }

        _context.Films.Add(film);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFilm), new { id = film.Id }, await GetFilmDtoAsync(film.Id));
    }

    /// <summary>
    /// Update an existing film (Admin only)
    /// </summary>
    /// <param name="id">Film ID</param>
    /// <param name="film">Updated film data</param>
    /// <returns>Updated film</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutFilm(int id, Film film)
    {
        if (id != film.Id)
        {
            return BadRequest();
        }

        var userId = GetCurrentUserId();
        if (userId != null)
        {
            film.AuthorId = userId;
        }

        _context.Entry(film).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Films.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    private async Task<FilmDto?> GetFilmDtoAsync(int id)
    {
        var film = await _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (film == null) return null;

        return new FilmDto
        {
            Id = film.Id,
            Name = film.Name,
            Description = film.Description,
            ReleaseDate = film.ReleaseDate,
            GenreId = film.GenreId,
            Genre = film.Genre != null ? new GenreDto
            {
                Id = film.Genre.Id,
                Name = film.Genre.Name,
                Description = film.Genre.Description
            } : null,
            ImageUrl = film.ImageUrl,
            AuthorId = film.AuthorId,
            Author = film.Author != null ? new UserBriefDto
            {
                Id = film.Author.Id,
                Name = film.Author.Name,
                Surname = film.Author.Surname
            } : null,
            AverageRating = film.Ratings?.Any() == true ? film.Ratings.Average(r => r.Value) : 0,
            RatingsCount = film.Ratings?.Count ?? 0
        };
    }

    /// <summary>
    /// Delete a film (Admin only)
    /// </summary>
    /// <param name="id">Film ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFilm(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        _context.Films.Remove(film);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get films created by current user
    /// </summary>
    /// <returns>List of user's films</returns>
    [HttpGet("my-films")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<FilmDto>>> GetMyFilms()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var films = await _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .Where(f => f.AuthorId == userId)
            .ToListAsync();

        return films.Select(f => new FilmDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
            ReleaseDate = f.ReleaseDate,
            GenreId = f.GenreId,
            Genre = f.Genre != null ? new GenreDto
            {
                Id = f.Genre.Id,
                Name = f.Genre.Name,
                Description = f.Genre.Description
            } : null,
            ImageUrl = f.ImageUrl,
            AuthorId = f.AuthorId,
            Author = f.Author != null ? new UserBriefDto
            {
                Id = f.Author.Id,
                Name = f.Author.Name,
                Surname = f.Author.Surname
            } : null,
            AverageRating = f.Ratings?.Any() == true ? f.Ratings.Average(r => r.Value) : 0,
            RatingsCount = f.Ratings?.Count ?? 0
        }).ToList();
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
