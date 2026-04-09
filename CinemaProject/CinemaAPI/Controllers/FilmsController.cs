using CinemaAPI.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly IFilmService _filmService;
    private readonly IAccountService _accountService;

    public FilmsController(IFilmService filmService, IAccountService accountService)
    {
        _filmService = filmService;
        _accountService = accountService;
    }

    // GET /api/Films
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FilmDto>>> GetFilms(
        [FromQuery] string? sortBy = null,
        [FromQuery] int? genreId = null,
        [FromQuery] string? search = null)
    {
        var films = await _filmService.GetAllFilmsAsync(sortBy, genreId, search);
        return Ok(films);
    }

    // GET /api/Films/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<FilmDto>> GetFilm(int id)
    {
        var film = await _filmService.GetFilmByIdAsync(id);
        if (film == null)
            return NotFound();

        return Ok(film);
    }

    // GET /api/Films/{id}/average-rating
    [HttpGet("{id}/average-rating")]
    [AllowAnonymous]
    public async Task<ActionResult<double>> GetAverageRating(int id)
    {
        try
        {
            var avg = await _filmService.GetAverageRatingAsync(id);
            return Ok(avg);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /api/Films
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<FilmDto>> PostFilm(
        [FromForm] FilmCreateDto dto,
        IFormFile? imageFile)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var film = await _filmService.CreateFilmAsync(dto, imageFile, dto.ExternalImageUrl, userId.Value);
            return CreatedAtAction(nameof(GetFilm), new { id = film.Id }, film);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/Films/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutFilm(
        int id,
        [FromForm] FilmUpdateDto dto,
        IFormFile? imageFile)
    {
        if (id != dto.Id)
            return BadRequest();

        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            await _filmService.UpdateFilmAsync(id, dto, imageFile, userId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE /api/Films/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFilm(int id)
    {
        try
        {
            await _filmService.DeleteFilmAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET /api/Films/my-films
    [HttpGet("my-films")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<FilmDto>>> GetMyFilms()
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var films = await _filmService.GetMyFilmsAsync(userId.Value);
        return Ok(films);
    }

    // GET /api/Films/{id}/comments
    [HttpGet("{id}/comments")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(int id)
    {
        try
        {
            var comments = await _filmService.GetCommentsAsync(id);
            return Ok(comments);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /api/Films/{id}/comments
    [HttpPost("{id}/comments")]
    [Authorize]
    public async Task<ActionResult<CommentDto>> PostComment(int id, [FromBody] CommentCreateDto dto)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var comment = await _filmService.AddCommentAsync(id, userId.Value, dto.Content);
            return Ok(comment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
