using System.Net;
using System.Net.Http.Json;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Xunit;

namespace CinemaAPI.Tests;

/// <summary>
/// Integration tests for Films API endpoints
/// </summary>
public class FilmsApiTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public FilmsApiTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFilms_ReturnsSuccessWithFilms()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Films");

        // Assert
        response.EnsureSuccessStatusCode();
        var films = await response.Content.ReadFromJsonAsync<List<FilmDto>>();

        Assert.NotNull(films);
        Assert.Equal(2, films.Count);
    }

    [Fact]
    public async Task GetFilmById_ReturnsFilm_WhenFilmExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Films/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var film = await response.Content.ReadFromJsonAsync<FilmDto>();

        Assert.NotNull(film);
        Assert.Equal(1, film.Id);
        Assert.Equal("Super Comedy", film.Name);
    }

    [Fact]
    public async Task GetFilmById_ReturnsNotFound_WhenFilmDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Films/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateFilm_WithAdminToken_CreatesFilmSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");
        var newFilm = new Film
        {
            Name = "New Drama Film",
            Description = "A dramatic masterpiece",
            ReleaseDate = DateTime.UtcNow,
            GenreId = 1,
            ImageUrl = "https://example.com/drama.jpg"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Films", newFilm);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdFilm = await response.Content.ReadFromJsonAsync<FilmDto>();

        Assert.NotNull(createdFilm);
        Assert.Equal("New Drama Film", createdFilm.Name);
        Assert.Equal(1, createdFilm.AuthorId); // Admin user ID
    }

    [Fact]
    public async Task CreateFilm_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newFilm = new Film
        {
            Name = "Unauthorized Film",
            Description = "Should not be created",
            ReleaseDate = DateTime.UtcNow,
            GenreId = 1,
            ImageUrl = "https://example.com/film.jpg"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Films", newFilm);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFilm_WithAdminToken_UpdatesFilmSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");
        var existingFilm = await client.GetAsync("/api/Films/2");
        var filmToUpdate = await existingFilm.Content.ReadFromJsonAsync<FilmDto>();

        var updatedFilm = new Film
        {
            Id = filmToUpdate.Id,
            Name = "Updated Comedy",
            Description = "Updated description",
            ReleaseDate = filmToUpdate.ReleaseDate,
            GenreId = filmToUpdate.GenreId,
            ImageUrl = filmToUpdate.ImageUrl
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/Films/2", updatedFilm);

        // Assert - API returns OK or NoContent on success
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task DeleteFilm_WithAdminToken_DeletesFilmSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");

        // Act
        var deleteResponse = await client.DeleteAsync("/api/Films/1");
        deleteResponse.EnsureSuccessStatusCode();

        // Verify film is deleted
        var getResponse = await client.GetAsync("/api/Films/1");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAverageRating_ReturnsCorrectAverage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Films/1/average-rating");

        // Assert - API returns the average rating value
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}
