using Minesweeper.Core.Models;

namespace Minesweeper.Data.Models;

public class Game
{
    public int Id { get; set; }
    
    // Foreign Key
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public GameSize Size { get; set; }
    public GameStatus Status { get; set; }
    public int TimeInSeconds { get; set; }
    public DateTime Date { get; set; }
    
    // JSON string or bitmask representing mine locations
    public string MineMap { get; set; } = string.Empty; 
}