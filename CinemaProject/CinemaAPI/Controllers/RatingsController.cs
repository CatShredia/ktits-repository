using CinemaAPI.Models;
using CinemaAPI.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly IAccountService _accountService;

    public RatingsController(IRatingService ratingService, IAccountService accountService)
    {
        _ratingService = ratingService;
        _accountService = accountService;
    }

    // GET /api/Ratings
    [HttpGet]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetRatings()
    {
        var ratings = await _ratingService.GetAllRatingsAsync();
        return Ok(ratings);
    }

    // GET /api/Ratings/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<RatingResponseDto>> GetRating(int id)
    {
        var rating = await _ratingService.GetRatingByIdAsync(id);
        if (rating == null)
            return NotFound();

        return Ok(rating);
    }

    // POST /api/Ratings
    [HttpPost]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<Rating>> PostRating(RatingCreateDto dto)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var rating = await _ratingService.CreateRatingAsync(dto, userId.Value);
            return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/Ratings/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,client")]
    public async Task<IActionResult> PutRating(int id, RatingUpdateDto dto)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("admin");

        try
        {
            await _ratingService.UpdateRatingAsync(id, dto, userId.Value, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // DELETE /api/Ratings/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,client")]
    public async Task<IActionResult> DeleteRating(int id)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("admin");

        try
        {
            await _ratingService.DeleteRatingAsync(id, userId.Value, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // GET /api/Ratings/film/{filmId}/my-rating
    [HttpGet("film/{filmId}/my-rating")]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<Rating>> GetMyRatingForFilm(int filmId)
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var rating = await _ratingService.GetMyRatingForFilmAsync(filmId, userId.Value);
            return Ok(rating);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("No rating found for this film");
        }
    }

    // GET /api/Ratings/my-ratings
    [HttpGet("my-ratings")]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetMyRatings()
    {
        var userId = _accountService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var ratings = await _ratingService.GetMyRatingsAsync(userId.Value);
        return Ok(ratings);
    }
}
