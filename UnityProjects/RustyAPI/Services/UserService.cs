using Microsoft.EntityFrameworkCore;
using RustyAPI.Database;
using RustyAPI.Database.DTOs;
using RustyAPI.Database.Models;

namespace RustyAPI.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(int userId);
    Task<UserProfileDto?> AddCoinsAsync(int userId, int coinsDelta);
    Task<UserProfileDto?> SaveProgressAsync(int userId, UpdateProgressDto dto);
    Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(int limit);
}

public class UserService : IUserService
{
    private readonly RustyDbContext _dbContext;

    public UserService(RustyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.LevelProgresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user == null ? null : ToProfileDto(user);
    }

    public async Task<UserProfileDto?> AddCoinsAsync(int userId, int coinsDelta)
    {
        var user = await _dbContext.Users
            .Include(u => u.LevelProgresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        user.Coins += Math.Max(0, coinsDelta);
        await _dbContext.SaveChangesAsync();
        return ToProfileDto(user);
    }

    public async Task<UserProfileDto?> SaveProgressAsync(int userId, UpdateProgressDto dto)
    {
        var user = await _dbContext.Users
            .Include(u => u.LevelProgresses)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;

        user.LastCompletedLevelIndex = Math.Max(user.LastCompletedLevelIndex, dto.LastCompletedLevelIndex);

        foreach (var level in dto.LevelProgresses)
        {
            if (string.IsNullOrWhiteSpace(level.LevelKey))
            {
                continue;
            }

            var stars = Math.Clamp(level.StarsCollected, 0, 3);
            var existing = user.LevelProgresses.FirstOrDefault(p => p.LevelKey == level.LevelKey);
            if (existing == null)
            {
                user.LevelProgresses.Add(new UserLevelProgress
                {
                    LevelKey = level.LevelKey,
                    LevelIndex = Math.Max(0, level.LevelIndex),
                    StarsCollected = stars,
                    Completed = level.Completed || stars >= 3,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.LevelIndex = Math.Max(existing.LevelIndex, level.LevelIndex);
                existing.StarsCollected = Math.Max(existing.StarsCollected, stars);
                existing.Completed = existing.Completed || level.Completed || existing.StarsCollected >= 3;
                existing.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync();
        return ToProfileDto(user);
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(int limit)
    {
        var safeLimit = Math.Clamp(limit, 1, 100);

        var users = await _dbContext.Users
            .Include(u => u.LevelProgresses)
            .OrderByDescending(u => u.Coins)
            .ThenByDescending(u => u.LastCompletedLevelIndex)
            .ThenByDescending(u => u.LevelProgresses.Sum(p => p.StarsCollected))
            .ThenBy(u => u.Username)
            .Take(safeLimit)
            .ToListAsync();

        return users.Select((user, index) => new LeaderboardEntryDto
        {
            Rank = index + 1,
            Username = user.Username,
            Coins = user.Coins,
            LastCompletedLevelIndex = user.LastCompletedLevelIndex,
            TotalStars = user.LevelProgresses.Sum(p => p.StarsCollected)
        }).ToList();
    }

    private static UserProfileDto ToProfileDto(User user)
    {
        var progress = user.LevelProgresses
            .OrderBy(p => p.LevelIndex)
            .ThenBy(p => p.LevelKey)
            .Select(p => new LevelProgressDto
            {
                LevelKey = p.LevelKey,
                LevelIndex = p.LevelIndex,
                StarsCollected = p.StarsCollected,
                Completed = p.Completed
            })
            .ToList();

        return new UserProfileDto
        {
            Id = user.Id,
            UserId = user.UserId,
            Username = user.Username,
            Coins = user.Coins,
            LastCompletedLevelIndex = user.LastCompletedLevelIndex,
            TotalStars = progress.Sum(p => p.StarsCollected),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            LevelProgresses = progress
        };
    }
}
