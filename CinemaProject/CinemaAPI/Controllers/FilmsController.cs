using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<Film>>> GetFilms(
        [FromQuery] string? sortBy = null,
        [FromQuery] int? genreId = null,
        [FromQuery] string? search = null)
    {
        var query = _context.Films
            .Include(f => f.Genre)
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

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get a specific film by ID
    /// </summary>
    /// <param name="id">Film ID</param>
    /// <returns>The film</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Film>> GetFilm(int id)
    {
        var film = await _context.Films
            .Include(f => f.Genre)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (film == null)
        {
            return NotFound();
        }

        return film;
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
    public async Task<ActionResult<Film>> PostFilm(Film film)
    {
        _context.Films.Add(film);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFilm), new { id = film.Id }, film);
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
}
