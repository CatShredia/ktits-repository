using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers;

// genre CRUD and get one
[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly DatabaseContext _context;

    public GenresController(DatabaseContext context)
    {
        _context = context;
    }

    // ! GetGenres - returns all genres list
    // GET /api/Genres (из CinemaBlazor через GenreService.GetAllGenresAsync)
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    // ! GetGenre - returns single genre by ID
    // GET /api/Genres/{id} (из CinemaBlazor через GenreService.GetGenreByIdAsync)
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre == null)
        {
            return NotFound();
        }

        return genre;
    }

    // ! PostGenre - creates new genre (admin only)
    // POST /api/Genres (из CinemaBlazor через GenreService.CreateGenreAsync)
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }

    // ! PutGenre - updates genre by ID (admin only)
    // PUT /api/Genres/{id} (из CinemaBlazor через GenreService.UpdateGenreAsync)
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutGenre(int id, Genre genre)
    {
        if (id != genre.Id)
        {
            return BadRequest();
        }

        _context.Entry(genre).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Genres.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // ! DeleteGenre - deletes genre by ID (admin only)
    // DELETE /api/Genres/{id} (из CinemaBlazor через GenreService.DeleteGenreAsync)
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
