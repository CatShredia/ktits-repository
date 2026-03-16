using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public RatingsController(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all ratings
    /// </summary>
    /// <returns>List of ratings</returns>
    [HttpGet]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
    {
        return await _context.Ratings
            .Include(r => r.Film)
            .ToListAsync();
    }

    /// <summary>
    /// Get a specific rating by ID (Client and Admin)
    /// </summary>
    /// <param name="id">Rating ID</param>
    /// <returns>The rating</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<Rating>> GetRating(int id)
    {
        var rating = await _context.Ratings
            .Include(r => r.Film)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rating == null)
        {
            return NotFound();
        }

        return rating;
    }

    /// <summary>
    /// Create a new rating (Admin and Client)
    /// </summary>
    /// <param name="rating">Rating data</param>
    /// <returns>Created rating</returns>
    [HttpPost]
    [Authorize(Roles = "admin,client")]
    public async Task<ActionResult<Rating>> PostRating(Rating rating)
    {
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating);
    }

    /// <summary>
    /// Update an existing rating (Admin only)
    /// </summary>
    /// <param name="id">Rating ID</param>
    /// <param name="rating">Updated rating data</param>
    /// <returns>Updated rating</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> PutRating(int id, Rating rating)
    {
        if (id != rating.Id)
        {
            return BadRequest();
        }

        _context.Entry(rating).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Ratings.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a rating (Admin only)
    /// </summary>
    /// <param name="id">Rating ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteRating(int id)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null)
        {
            return NotFound();
        }

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
