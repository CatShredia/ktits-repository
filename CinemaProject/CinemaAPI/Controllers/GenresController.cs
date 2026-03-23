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

    // ! Genres get
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    // ! Genres one
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

    // ! Genre creare new
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }

    // ! Genre update
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

    // ! Genre delete
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
