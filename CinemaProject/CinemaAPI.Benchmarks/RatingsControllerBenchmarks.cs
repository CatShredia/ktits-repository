using BenchmarkDotNet.Attributes;
using CinemaAPI.Controllers;
using CinemaAPI.Data;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Benchmarks;

[MemoryDiagnoser]
[HtmlExporter, MarkdownExporter]
public class RatingsControllerBenchmarks
{
    private RatingsController _controller = null!;
    private DatabaseContext _context = null!;

    [GlobalSetup]
    public void Setup()
    {
        _context = CreateContext();
        _controller = new RatingsController(_context);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    private DatabaseContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase($"BenchmarkDb_Ratings_{Guid.NewGuid()}")
            .Options;

        var context = new DatabaseContext(options);
        SeedData(context);
        return context;
    }

    private void SeedData(DatabaseContext context)
    {
        // Roles
        var adminRole = new Role { Id = 1, Name = "admin", Description = "Administrator" };
        var clientRole = new Role { Id = 2, Name = "client", Description = "Client" };
        context.Roles.AddRange(adminRole, clientRole);

        // Users
        var adminUser = new User
        {
            Id = 1,
            Name = "Admin",
            Surname = "User",
            Email = "admin@cinema.com",
            Description = "Test Admin",
            Gender = "Other",
            RoleId = 1,
            Login = new Login { Id = 1, LoginValue = "admin", PasswordHash = "hashed_password" }
        };

        var clientUser = new User
        {
            Id = 2,
            Name = "John",
            Surname = "Doe",
            Email = "john@cinema.com",
            Description = "Test Client",
            Gender = "Male",
            RoleId = 2,
            Login = new Login { Id = 2, LoginValue = "client", PasswordHash = "hashed_password" }
        };

        var clientUser2 = new User
        {
            Id = 3,
            Name = "Jane",
            Surname = "Smith",
            Email = "jane@cinema.com",
            Description = "Test Client 2",
            Gender = "Female",
            RoleId = 2,
            Login = new Login { Id = 3, LoginValue = "client2", PasswordHash = "hashed_password" }
        };

        context.Users.AddRange(adminUser, clientUser, clientUser2);

        // Genres
        var actionGenre = new Genre { Id = 1, Name = "Action", Description = "Action movies" };
        var comedyGenre = new Genre { Id = 2, Name = "Comedy", Description = "Comedy movies" };
        var dramaGenre = new Genre { Id = 3, Name = "Drama", Description = "Drama movies" };
        context.Genres.AddRange(actionGenre, comedyGenre, dramaGenre);

        // Films
        var films = new List<Film>
        {
            new Film { Id = 1, Name = "The Matrix", Description = "A computer hacker learns about the true nature of reality", ReleaseDate = new DateTime(1999, 3, 31), GenreId = 1, AuthorId = 1, ImageUrl = "matrix.jpg" },
            new Film { Id = 2, Name = "Inception", Description = "A thief who steals corporate secrets through dream-sharing technology", ReleaseDate = new DateTime(2010, 7, 16), GenreId = 1, AuthorId = 1, ImageUrl = "inception.jpg" },
            new Film { Id = 3, Name = "The Hangover", Description = "Three buddies wake up from a bachelor party in Las Vegas", ReleaseDate = new DateTime(2009, 6, 2), GenreId = 2, AuthorId = 2, ImageUrl = "hangover.jpg" },
            new Film { Id = 4, Name = "Superbad", Description = "Two co-dependent high school seniors are forced to deal with separation anxiety", ReleaseDate = new DateTime(2007, 8, 17), GenreId = 2, AuthorId = 2, ImageUrl = "superbad.jpg" },
            new Film { Id = 5, Name = "The Shawshank Redemption", Description = "Two imprisoned men bond over a number of years", ReleaseDate = new DateTime(1994, 9, 23), GenreId = 3, AuthorId = 3, ImageUrl = "shawshank.jpg" }
        };
        context.Films.AddRange(films);

        // Ratings
        var ratings = new List<Rating>
        {
            new Rating { Id = 1, Value = 9, FilmId = 1, AuthorId = 2 },
            new Rating { Id = 2, Value = 8, FilmId = 1, AuthorId = 3 },
            new Rating { Id = 3, Value = 9, FilmId = 2, AuthorId = 2 },
            new Rating { Id = 4, Value = 8, FilmId = 2, AuthorId = 3 },
            new Rating { Id = 5, Value = 7, FilmId = 3, AuthorId = 2 },
            new Rating { Id = 6, Value = 8, FilmId = 4, AuthorId = 3 },
            new Rating { Id = 7, Value = 9, FilmId = 5, AuthorId = 2 },
            new Rating { Id = 8, Value = 9, FilmId = 5, AuthorId = 3 }
        };
        context.Ratings.AddRange(ratings);

        context.SaveChanges();
    }

    // GET /api/Ratings
    [Benchmark]
    public async Task<List<RatingResponseDto>> GetAllRatings()
    {
        var result = await _controller.GetRatings();
        return result.Value!.ToList();
    }

    // GET /api/Ratings/{id}
    [Benchmark]
    public async Task<RatingResponseDto?> GetRatingById()
    {
        var result = await _controller.GetRating(1);
        return result.Value;
    }

    // POST /api/Ratings
    [Benchmark]
    public async Task<bool> CreateRating()
    {
        var dto = new RatingCreateDto { Value = 8, FilmId = 3 };
        var result = await _controller.PostRating(dto);
        return true;
    }

    // PUT /api/Ratings/{id}
    [Benchmark]
    public async Task<bool> UpdateRating()
    {
        var dto = new RatingUpdateDto { Value = 7 };
        var result = await _controller.PutRating(1, dto);
        return true;
    }

    // DELETE /api/Ratings/{id}
    [Benchmark]
    public async Task<bool> DeleteRating()
    {
        var result = await _controller.DeleteRating(1);
        return true;
    }

    // GET /api/Ratings/film/{filmId}/my-rating
    [Benchmark]
    public async Task<bool> GetMyRatingForFilm()
    {
        var result = await _controller.GetMyRatingForFilm(1);
        return true;
    }
}
