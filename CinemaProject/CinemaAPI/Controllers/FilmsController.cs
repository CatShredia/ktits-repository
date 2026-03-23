using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CinemaAPI.Controllers;

// Film get/get one, Average film rating get, Create New Film, DeleteFilm
[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IImageService _imageService;

    public FilmsController(DatabaseContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    // ! Films get
    // Film search, sort, filter by query
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

        if (genreId.HasValue)
        {
            query = query.Where(f => f.GenreId == genreId.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(f => f.Name.Contains(search));
        }

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

    // ! Film one get
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

    // ! Average film rating get
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

    // ! Create New Film, only admins
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<FilmDto>> PostFilm(
        [FromForm] FilmCreateDto dto,
        IFormFile? imageFile)
    {
        var imageUrl = await ProcessImageAsync(imageFile, dto.ExternalImageUrl);

        var film = new Film
        {
            Name = dto.Name,
            Description = dto.Description,
            ReleaseDate = dto.ReleaseDate,
            GenreId = dto.GenreId,
            ImageUrl = imageUrl
        };

        var userId = GetCurrentUserId();
        if (userId != null)
        {
            film.AuthorId = userId;
        }

        _context.Films.Add(film);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFilm), new { id = film.Id }, await GetFilmDtoAsync(film.Id));
    }

    // ! upload image
    private async Task<string?> ProcessImageAsync(IFormFile? imageFile, string? externalImageUrl)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                return await _imageService.SaveImageAsync(imageFile);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(imageFile), ex.Message);
                return null;
            }
        }

        if (!string.IsNullOrEmpty(externalImageUrl))
        {
            if (Uri.TryCreate(externalImageUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https"))
            {
                return externalImageUrl;
            }
            ModelState.AddModelError(nameof(externalImageUrl), "Invalid URL format");
        }

        return null;
    }

    // ! Update Film, only admins
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutFilm(
        int id,
        [FromForm] FilmUpdateDto dto,
        IFormFile? imageFile)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var film = await _context.Films.FindAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        film.Name = dto.Name;
        film.Description = dto.Description;
        film.ReleaseDate = dto.ReleaseDate;
        film.GenreId = dto.GenreId;

        if (imageFile != null && imageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(film.ImageUrl))
            {
                _imageService.DeleteImage(film.ImageUrl);
            }

            try
            {
                film.ImageUrl = await _imageService.SaveImageAsync(imageFile);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(imageFile), ex.Message);
                return BadRequest(ModelState);
            }
        }
        else if (dto.RemoveImage)
        {
            if (!string.IsNullOrEmpty(film.ImageUrl))
            {
                _imageService.DeleteImage(film.ImageUrl);
            }
            film.ImageUrl = null;
        }
        else if (!string.IsNullOrEmpty(dto.ExternalImageUrl))
        {
            if (Uri.TryCreate(dto.ExternalImageUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https"))
            {
                if (!string.IsNullOrEmpty(film.ImageUrl) && film.ImageUrl.StartsWith("/images/"))
                {
                    _imageService.DeleteImage(film.ImageUrl);
                }
                film.ImageUrl = dto.ExternalImageUrl;
            }
            else
            {
                ModelState.AddModelError(nameof(dto.ExternalImageUrl), "Invalid URL format");
                return BadRequest(ModelState);
            }
        }

        var userId = GetCurrentUserId();
        if (userId != null)
        {
            film.AuthorId = userId;
        }

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

    // ! Film one get without endpoint
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

    // ! Delete Film 
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFilm(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(film.ImageUrl))
        {
            _imageService.DeleteImage(film.ImageUrl);
        }

        _context.Films.Remove(film);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // MyFilms get
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
