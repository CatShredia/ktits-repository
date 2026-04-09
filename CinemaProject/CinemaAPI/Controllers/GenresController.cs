using CinemaAPI.Models;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    // GET /api/Genres
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        var genres = await _genreService.GetAllGenresAsync();
        return Ok(genres);
    }

    // GET /api/Genres/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        if (genre == null)
            return NotFound();

        return Ok(genre);
    }

    // POST /api/Genres
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<Genre>> PostGenre(Genre genre)
    {
        var created = await _genreService.CreateGenreAsync(genre);
        return CreatedAtAction(nameof(GetGenre), new { id = created.Id }, created);
    }

    // PUT /api/Genres/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutGenre(int id, Genre genre)
    {
        try
        {
            await _genreService.UpdateGenreAsync(id, genre);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    // DELETE /api/Genres/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        try
        {
            await _genreService.DeleteGenreAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
