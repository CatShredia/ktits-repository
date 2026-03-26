namespace ArkanoidAPI.Models;

public enum SkinType
{
    Platform = 0,
    Ball = 1
}

public enum SkinRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4
}

public class Skin
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public SkinType SkinType { get; set; }

    public SkinRarity Rarity { get; set; } = SkinRarity.Common;

    public int Price { get; set; } = 0;

    public string TexturePath { get; set; } = string.Empty;

    public string? PrefabPath { get; set; }

    public bool IsStarter { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserSkin> UserSkins { get; set; } = new List<UserSkin>();

    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
