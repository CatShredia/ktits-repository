namespace ArkanoidAPI.Models;

public class User
{
    public int Id { get; set; }

    public string UserId { get; set; } = Guid.NewGuid().ToString();

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int Coins { get; set; } = 100;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserSkin> UserSkins { get; set; } = new List<UserSkin>();

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
