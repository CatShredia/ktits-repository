namespace ArkanoidAPI.Models;

public class UserSkin
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SkinId { get; set; }

    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;

    public AcquisitionMethod AcquisitionMethod { get; set; } = AcquisitionMethod.Purchase;

    public bool IsEquipped { get; set; } = false;

    public User? User { get; set; }
    public Skin? Skin { get; set; }
}

public enum AcquisitionMethod
{
    Purchase = 0,
    Reward = 1,
    Gift = 2,
    Starter = 3
}
