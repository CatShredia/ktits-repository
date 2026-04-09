using CinemaAPI.Data;
using CinemaAPI.Data.Models;
using CinemaAPI.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services;

public interface IFilmService
{
    Task<IEnumerable<FilmDto>> GetAllFilmsAsync(string? sortBy, int? genreId, string? search);
    Task<FilmDto?> GetFilmByIdAsync(int id);
    Task<double> GetAverageRatingAsync(int filmId);
    Task<FilmDto> CreateFilmAsync(FilmCreateDto dto, IFormFile? imageFile, string? externalImageUrl, int authorId);
    Task UpdateFilmAsync(int id, FilmUpdateDto dto, IFormFile? imageFile, int authorId);
    Task DeleteFilmAsync(int id);
    Task<IEnumerable<FilmDto>> GetMyFilmsAsync(int authorId);
    Task<IEnumerable<CommentDto>> GetCommentsAsync(int filmId);
    Task<CommentDto> AddCommentAsync(int filmId, int userId, string content);
}

public class FilmService : IFilmService
{
    private readonly DatabaseContext _context;
    private readonly IImageService _imageService;

    public FilmService(DatabaseContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    public async Task<IEnumerable<FilmDto>> GetAllFilmsAsync(string? sortBy, int? genreId, string? search)
    {
        var query = _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .Include(f => f.CommentsConversation)
            .AsQueryable();

        if (genreId.HasValue)
            query = query.Where(f => f.GenreId == genreId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(f => f.Name.Contains(search));

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
        return films.Select(MapToFilmDto).ToList();
    }

    public async Task<FilmDto?> GetFilmByIdAsync(int id)
    {
        var film = await _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .Include(f => f.CommentsConversation)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (film == null)
            return null;

        return MapToFilmDto(film);
    }

    public async Task<double> GetAverageRatingAsync(int filmId)
    {
        var film = await _context.Films.FindAsync(filmId);
        if (film == null)
            throw new KeyNotFoundException("Film not found");

        var ratings = await _context.Ratings
            .Where(r => r.FilmId == filmId)
            .ToListAsync();

        if (ratings.Count == 0)
            return 0.0;

        return ratings.Average(r => r.Value);
    }

    public async Task<FilmDto> CreateFilmAsync(FilmCreateDto dto, IFormFile? imageFile, string? externalImageUrl, int authorId)
    {
        var imageUrl = await ProcessImageAsync(imageFile, externalImageUrl);

        var film = new Film
        {
            Name = dto.Name,
            Description = dto.Description,
            ReleaseDate = dto.ReleaseDate,
            GenreId = dto.GenreId,
            ImageUrl = imageUrl,
            AuthorId = authorId
        };

        _context.Films.Add(film);
        await _context.SaveChangesAsync();

        var createdFilm = await GetFilmByIdAsync(film.Id);
        return createdFilm!;
    }

    public async Task UpdateFilmAsync(int id, FilmUpdateDto dto, IFormFile? imageFile, int authorId)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null)
            throw new KeyNotFoundException("Film not found");

        film.Name = dto.Name;
        film.Description = dto.Description;
        film.ReleaseDate = dto.ReleaseDate;
        film.GenreId = dto.GenreId;

        if (imageFile != null && imageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(film.ImageUrl))
                _imageService.DeleteImage(film.ImageUrl);

            try
            {
                film.ImageUrl = await _imageService.SaveImageAsync(imageFile);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(imageFile));
            }
        }
        else if (dto.RemoveImage)
        {
            if (!string.IsNullOrEmpty(film.ImageUrl))
                _imageService.DeleteImage(film.ImageUrl);
            film.ImageUrl = null;
        }
        else if (!string.IsNullOrEmpty(dto.ExternalImageUrl))
        {
            if (Uri.TryCreate(dto.ExternalImageUrl, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == "http" || uriResult.Scheme == "https"))
            {
                if (!string.IsNullOrEmpty(film.ImageUrl) && film.ImageUrl.StartsWith("/images/"))
                    _imageService.DeleteImage(film.ImageUrl);
                film.ImageUrl = dto.ExternalImageUrl;
            }
            else
            {
                throw new ArgumentException("Invalid URL format", nameof(dto.ExternalImageUrl));
            }
        }

        film.AuthorId = authorId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Films.Any(e => e.Id == id))
                throw new KeyNotFoundException("Film not found");
            throw;
        }
    }

    public async Task DeleteFilmAsync(int id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null)
            throw new KeyNotFoundException("Film not found");

        if (!string.IsNullOrEmpty(film.ImageUrl))
            _imageService.DeleteImage(film.ImageUrl);

        _context.Films.Remove(film);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<FilmDto>> GetMyFilmsAsync(int authorId)
    {
        var films = await _context.Films
            .Include(f => f.Genre)
            .Include(f => f.Author)
            .Include(f => f.Ratings)
            .Include(f => f.CommentsConversation)
            .Where(f => f.AuthorId == authorId)
            .ToListAsync();

        return films.Select(MapToFilmDto).ToList();
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAsync(int filmId)
    {
        var film = await _context.Films
            .Include(f => f.CommentsConversation)
            .FirstOrDefaultAsync(f => f.Id == filmId);

        if (film == null)
            throw new KeyNotFoundException("Film not found");

        if (film.CommentsConversationId == null)
            return new List<CommentDto>();

        var messages = await _context.Messages
            .Where(m => m.ConversationId == film.CommentsConversationId)
            .OrderBy(m => m.CreatedAt)
            .Include(m => m.Sender)
            .ToListAsync();

        return messages.Select(m => new CommentDto
        {
            Id = m.Id,
            Content = m.Content,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            SenderId = m.SenderId,
            Sender = new UserBriefDto
            {
                Id = m.Sender.Id,
                Name = m.Sender.Name,
                Surname = m.Sender.Surname
            }
        }).ToList();
    }

    public async Task<CommentDto> AddCommentAsync(int filmId, int userId, string content)
    {
        var film = await _context.Films
            .Include(f => f.CommentsConversation)
            .FirstOrDefaultAsync(f => f.Id == filmId);

        if (film == null)
            throw new KeyNotFoundException("Film not found");

        if (film.CommentsConversationId == null)
        {
            var conversation = new Conversation
            {
                ConversationTypeId = 4,
                CreatedAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            film.CommentsConversationId = conversation.Id;
            await _context.SaveChangesAsync();

            _context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = userId,
                RoleId = 4
            });
            await _context.SaveChangesAsync();
        }

        var message = new Message
        {
            ConversationId = film.CommentsConversationId.Value,
            SenderId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var sender = await _context.Users.FindAsync(userId);

        return new CommentDto
        {
            Id = message.Id,
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            SenderId = message.SenderId,
            Sender = new UserBriefDto
            {
                Id = sender!.Id,
                Name = sender.Name,
                Surname = sender.Surname
            }
        };
    }

    #region Helper Methods

    private async Task<string?> ProcessImageAsync(IFormFile? imageFile, string? externalImageUrl)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                return await _imageService.SaveImageAsync(imageFile);
            }
            catch (ArgumentException)
            {
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
        }

        return null;
    }

    private FilmDto MapToFilmDto(Film film)
    {
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
            RatingsCount = film.Ratings?.Count ?? 0,
            CommentsCount = film.CommentsConversation?.Messages?.Count ?? 0
        };
    }

    #endregion
}
