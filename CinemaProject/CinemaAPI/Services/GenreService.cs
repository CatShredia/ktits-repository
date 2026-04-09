using CinemaAPI.Data;
using CinemaAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Services;

public interface IGenreService
{
    Task<IEnumerable<Genre>> GetAllGenresAsync();
    Task<Genre?> GetGenreByIdAsync(int id);
    Task<Genre> CreateGenreAsync(Genre genre);
    Task UpdateGenreAsync(int id, Genre genre);
    Task DeleteGenreAsync(int id);
}

public class GenreService : IGenreService
{
    private readonly DatabaseContext _context;

    public GenreService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Genre>> GetAllGenresAsync()
    {
        return await _context.Genres.ToListAsync();
    }

    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        return await _context.Genres.FindAsync(id);
    }

    public async Task<Genre> CreateGenreAsync(Genre genre)
    {
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre;
    }

    public async Task UpdateGenreAsync(int id, Genre genre)
    {
        if (id != genre.Id)
            throw new ArgumentException("ID mismatch");

        _context.Entry(genre).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Genres.Any(e => e.Id == id))
                throw new KeyNotFoundException("Genre not found");
            throw;
        }
    }

    public async Task DeleteGenreAsync(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
            throw new KeyNotFoundException("Genre not found");

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
    }
}
