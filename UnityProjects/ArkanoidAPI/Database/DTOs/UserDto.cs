namespace ArkanoidAPI.Database.DTOs;

public class UserDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public int Coins { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastLoginAt { get; set; }
}
