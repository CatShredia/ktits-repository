using Minesweeper.Core.Models;
using Minesweeper.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Minesweeper.Data.Repositories;

public interface IGameRepository
{
    Task SaveGameAsync(int userId, GameSize size, GameStatus status, int timeSec, bool[,] mineMap);
    Task<List<Game>> GetUserHistoryAsync(int userId);
    Task<List<Game>> GetLeaderboardAsync(GameSize size, int count = 10);
}

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;
    public GameRepository(AppDbContext context) => _context = context;

    public async Task SaveGameAsync(int userId, GameSize size, GameStatus status, int timeSec, bool[,] mineMap)
    {
        var game = new Game
        {
            UserId = userId,
            Size = size,
            Status = status,
            TimeInSeconds = timeSec,
            Date = DateTime.UtcNow,
            MineMap = MineMapCodec.Encode(mineMap)
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Game>> GetUserHistoryAsync(int userId)
        => await _context.Games.Where(g => g.UserId == userId).OrderByDescending(g => g.Date).ToListAsync();

    public async Task<List<Game>> GetLeaderboardAsync(GameSize size, int count = 10)
        => await _context.Games
            .Where(g => g.Size == size && g.Status == GameStatus.Win)
            .OrderBy(g => g.TimeInSeconds)
            .Take(count)
            .ToListAsync();
}