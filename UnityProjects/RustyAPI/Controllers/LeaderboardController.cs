using Microsoft.AspNetCore.Mvc;
using RustyAPI.Database.DTOs;
using RustyAPI.Services;

namespace RustyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly IUserService _userService;

    public LeaderboardController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int limit = 10)
    {
        var leaderboard = await _userService.GetLeaderboardAsync(limit);
        return Ok(leaderboard);
    }
}
