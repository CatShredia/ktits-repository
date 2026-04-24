namespace RustyAPI.Database.DTOs;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Coins { get; set; }
    public int LastCompletedLevelIndex { get; set; }
    public int TotalStars { get; set; }
}
