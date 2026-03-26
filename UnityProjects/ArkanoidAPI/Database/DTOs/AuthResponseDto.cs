namespace ArkanoidAPI.Database.DTOs;

public class AuthResponseDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}
