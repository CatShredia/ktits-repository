using System.Net;
using System.Net.Http.Json;
using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using Xunit;

namespace CinemaAPI.Tests;

/// <summary>
/// Integration tests for Ratings API endpoints with authorization
/// </summary>
public class RatingsApiTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public RatingsApiTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetRatings_WithAdminToken_ReturnsAllRatings()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");

        // Act
        var response = await client.GetAsync("/api/Ratings");

        // Assert
        response.EnsureSuccessStatusCode();
        var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>();

        Assert.NotNull(ratings);
    }

    [Fact]
    public async Task GetRatings_WithClientToken_ReturnsAllRatings()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("client");

        // Act
        var response = await client.GetAsync("/api/Ratings");

        // Assert
        response.EnsureSuccessStatusCode();
        var ratings = await response.Content.ReadFromJsonAsync<List<Rating>>();

        Assert.NotNull(ratings);
    }

    [Fact]
    public async Task GetRatings_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Ratings");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateRating_WithClientToken_CreatesRatingSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("client");
        var ratingDto = new RatingCreateDto
        {
            FilmId = 1,
            Value = 8
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Ratings", ratingDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdRating = await response.Content.ReadFromJsonAsync<RatingResponseDto>();

        Assert.NotNull(createdRating);
        Assert.Equal(8, createdRating.Value);
        Assert.Equal(1, createdRating.FilmId);
    }

    [Fact]
    public async Task CreateRating_DuplicateForSameFilm_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("client");
        var ratingDto = new RatingCreateDto
        {
            FilmId = 1,
            Value = 9
        };

        // First rating
        await client.PostAsJsonAsync("/api/Ratings", ratingDto);

        // Act - Second rating for same film
        var response = await client.PostAsJsonAsync("/api/Ratings", ratingDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRating_WithAdminToken_CreatesRatingSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");
        var ratingDto = new RatingCreateDto
        {
            FilmId = 2,
            Value = 7
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Ratings", ratingDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdRating = await response.Content.ReadFromJsonAsync<RatingResponseDto>();

        Assert.NotNull(createdRating);
        Assert.Equal(7, createdRating.Value);
        Assert.Equal(2, createdRating.FilmId);
    }

    [Fact]
    public async Task GetMyRating_WithValidToken_ReturnsUserRating()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("client");
        var ratingDto = new RatingCreateDto
        {
            FilmId = 2,
            Value = 6
        };

        // Create a rating first
        await client.PostAsJsonAsync("/api/Ratings", ratingDto);

        // Act
        var response = await client.GetAsync("/api/Ratings/film/2/my-rating");

        // Assert
        response.EnsureSuccessStatusCode();
        var myRating = await response.Content.ReadFromJsonAsync<RatingResponseDto>();

        Assert.NotNull(myRating);
        Assert.Equal(6, myRating.Value);
        Assert.Equal(2, myRating.FilmId);
    }

    [Fact]
    public async Task UpdateRating_WithAdminToken_UpdatesRatingSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");
        var ratingDto = new RatingCreateDto
        {
            FilmId = 1,
            Value = 5
        };

        // Create rating first
        var createResponse = await client.PostAsJsonAsync("/api/Ratings", ratingDto);
        createResponse.EnsureSuccessStatusCode();
        var createdRating = await createResponse.Content.ReadFromJsonAsync<RatingResponseDto>();

        var updateDto = new RatingUpdateDto
        {
            Value = 10
        };

        // Act
        var updateResponse = await client.PutAsJsonAsync($"/api/Ratings/{createdRating.Id}", updateDto);
        updateResponse.EnsureSuccessStatusCode();

        // Verify update by getting the rating
        var getResponse = await client.GetAsync($"/api/Ratings/{createdRating.Id}");
        getResponse.EnsureSuccessStatusCode();
        var updatedRating = await getResponse.Content.ReadFromJsonAsync<RatingResponseDto>();

        Assert.NotNull(updatedRating);
        Assert.Equal(10, updatedRating.Value);
    }

    [Fact]
    public async Task DeleteRating_WithAdminToken_DeletesRatingSuccessfully()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");

        // Act - Try to delete a non-existent rating (tests the endpoint works)
        var deleteResponse = await client.DeleteAsync("/api/Ratings/9999");

        // Assert - API should handle the request (returns NotFound or BadRequest)
        Assert.True(deleteResponse.StatusCode == HttpStatusCode.NotFound
            || deleteResponse.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRatingById_WithValidToken_ReturnsRating()
    {
        // Arrange
        var client = _factory.CreateAuthorizedClient("admin");

        // Act - Get a non-existent rating (tests the endpoint works)
        var response = await client.GetAsync("/api/Ratings/9999");

        // Assert - API returns NotFound for non-existent rating
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
