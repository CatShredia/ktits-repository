namespace RustyAPI.Database.Models;

public class User
{
    public int Id { get; set; }
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int Coins { get; set; }
    public int LastCompletedLevelIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserLevelProgress> LevelProgresses { get; set; } = new List<UserLevelProgress>();
}
