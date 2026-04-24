namespace RustyAPI.Database.DTOs;

public class UserProfileDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Coins { get; set; }
    public int LastCompletedLevelIndex { get; set; }
    public int TotalStars { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public List<LevelProgressDto> LevelProgresses { get; set; } = new();
}
