namespace ArkanoidAPI.Models;

public class Purchase
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SkinId { get; set; }

    public int Price { get; set; }

    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Skin? Skin { get; set; }
}
