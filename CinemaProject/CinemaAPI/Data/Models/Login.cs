namespace CinemaAPI.Data.Models;

public class Login
{
    public int Id { get; set; }
    public string LoginValue { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
}
