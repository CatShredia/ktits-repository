namespace RustyAPI.Database.Models;

public class UserLevelProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string LevelKey { get; set; } = string.Empty;
    public int LevelIndex { get; set; }
    public int StarsCollected { get; set; }
    public bool Completed { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
