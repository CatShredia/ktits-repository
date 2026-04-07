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

    // ! GetFilms - returns film list with search, sort, and filter by genre
    // GET /api/Films (из CinemaBlazor через FilmService.GetAllFilmsAsync)
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

    // ! GetFilm - returns single film by ID with full details
    // GET /api/Films/{id} (из CinemaBlazor через FilmService.GetFilmByIdAsync)
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

    // ! GetAverageRating - returns average rating for a film
    // GET /api/Films/{id}/average-rating (из CinemaBlazor через FilmService.GetAverageRatingAsync)
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

    // ! PostFilm - creates new film (admin only), supports image upload
    // POST /api/Films (из CinemaBlazor через FilmService.CreateFilmAsync)
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<FilmDto>> PostFilm(
        [FromForm] FilmCreateDto dto,
        IFormFile? imageFile)
    {
        Console.WriteLine($"[API] PostFilm called");
        Console.WriteLine($"[API] dto.Name={dto.Name}, dto.GenreId={dto.GenreId}");
        Console.WriteLine($"[API] dto.ExternalImageUrl={dto.ExternalImageUrl ?? "null"}");
        Console.WriteLine($"[API] imageFile={(imageFile != null ? $"{imageFile.FileName} ({imageFile.ContentType}, {imageFile.Length} bytes)" : "null")}");

        var imageUrl = await ProcessImageAsync(imageFile, dto.ExternalImageUrl);

        Console.WriteLine($"[API] ProcessedImageUrl={imageUrl ?? "null"}");

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

        Console.WriteLine($"[API] Film created with Id={film.Id}");

        return CreatedAtAction(nameof(GetFilm), new { id = film.Id }, await GetFilmDtoAsync(film.Id));
    }

    // ! ProcessImageAsync - handles image upload (file or external URL)
    // вызывается внутри PostFilm и PutFilm методов этого контроллера
    private async Task<string?> ProcessImageAsync(IFormFile? imageFile, string? externalImageUrl)
    {
        Console.WriteLine($"[API] ProcessImageAsync: imageFile={(imageFile != null ? $"{imageFile.FileName}, {imageFile.Length} bytes" : "null")}, externalImageUrl={externalImageUrl ?? "null"}");

        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                var savedPath = await _imageService.SaveImageAsync(imageFile);
                Console.WriteLine($"[API] Image saved to: {savedPath}");
                return savedPath;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[API] Image save error: {ex.Message}");
                ModelState.AddModelError(nameof(imageFile), ex.Message);
                return null;
            }
        }

        if (!string.IsNullOrEmpty(externalImageUrl))
        {
            if (Uri.TryCreate(externalImageUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https"))
            {
                Console.WriteLine($"[API] Using external URL: {externalImageUrl}");
                return externalImageUrl;
            }
            Console.WriteLine($"[API] Invalid external URL: {externalImageUrl}");
            ModelState.AddModelError(nameof(externalImageUrl), "Invalid URL format");
        }

        return null;
    }

    // ! PutFilm - updates film by ID (admin only), supports image update
    // PUT /api/Films/{id} (из CinemaBlazor через FilmService.UpdateFilmAsync)
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutFilm(
        int id,
        [FromForm] FilmUpdateDto dto,
        IFormFile? imageFile)
    {
        Console.WriteLine($"[API] PutFilm called: id={id}");
        Console.WriteLine($"[API] dto.Name={dto.Name}, dto.GenreId={dto.GenreId}");
        Console.WriteLine($"[API] dto.ExternalImageUrl={dto.ExternalImageUrl ?? "null"}, dto.RemoveImage={dto.RemoveImage}");
        Console.WriteLine($"[API] imageFile={(imageFile != null ? $"{imageFile.FileName} ({imageFile.ContentType}, {imageFile.Length} bytes)" : "null")}");

        if (id != dto.Id)
        {
            Console.WriteLine($"[API] ID mismatch: {id} != {dto.Id}");
            return BadRequest();
        }

        var film = await _context.Films.FindAsync(id);
        if (film == null)
        {
            Console.WriteLine($"[API] Film not found: {id}");
            return NotFound();
        }

        film.Name = dto.Name;
        film.Description = dto.Description;
        film.ReleaseDate = dto.ReleaseDate;
        film.GenreId = dto.GenreId;

        if (imageFile != null && imageFile.Length > 0)
        {
            Console.WriteLine($"[API] Processing uploaded image file");
            if (!string.IsNullOrEmpty(film.ImageUrl))
            {
                _imageService.DeleteImage(film.ImageUrl);
            }

            try
            {
                film.ImageUrl = await _imageService.SaveImageAsync(imageFile);
                Console.WriteLine($"[API] Image saved to: {film.ImageUrl}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[API] Image save error: {ex.Message}");
                ModelState.AddModelError(nameof(imageFile), ex.Message);
                return BadRequest(ModelState);
            }
        }
        else if (dto.RemoveImage)
        {
            Console.WriteLine($"[API] Removing image");
            if (!string.IsNullOrEmpty(film.ImageUrl))
            {
                _imageService.DeleteImage(film.ImageUrl);
            }
            film.ImageUrl = null;
        }
        else if (!string.IsNullOrEmpty(dto.ExternalImageUrl))
        {
            Console.WriteLine($"[API] Setting external image URL: {dto.ExternalImageUrl}");
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
        else
        {
            Console.WriteLine($"[API] No image change - keeping existing image: {film.ImageUrl ?? "null"}");
        }

        var userId = GetCurrentUserId();
        if (userId != null)
        {
            film.AuthorId = userId;
        }

        try
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"[API] Film updated successfully");
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

    // ! GetFilmDtoAsync - helper to convert Film entity to FilmDto
    // вызывается внутри PostFilm метода этого контроллера
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

    // ! DeleteFilm - deletes film by ID (admin only), also deletes associated image
    // DELETE /api/Films/{id} (из CinemaBlazor через FilmService.DeleteFilmAsync)
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

    // ! GetMyFilms - returns films created by current user (admin only)
    // GET /api/Films/my-films (из CinemaBlazor через FilmService.GetMyFilmsAsync)
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

    // ! GetCurrentUserId - extracts user ID from JWT token claims
    // вызывается внутри всех методов этого контроллера
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
