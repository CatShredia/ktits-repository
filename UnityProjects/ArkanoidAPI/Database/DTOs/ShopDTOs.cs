namespace ArkanoidAPI.Database.DTOs;

public class SkinDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string SkinType { get; set; } = string.Empty;

    public string Rarity { get; set; } = string.Empty;

    public int Price { get; set; }

    public string TexturePath { get; set; } = string.Empty;

    public string? PrefabPath { get; set; }

    public bool IsStarter { get; set; }

    public bool IsActive { get; set; }
}

public class UserSkinDto
{
    public int Id { get; set; }

    public int SkinId { get; set; }

    public string SkinName { get; set; } = string.Empty;

    public string SkinType { get; set; } = string.Empty;

    public DateTime AcquiredAt { get; set; }

    public bool IsEquipped { get; set; }

    public string AcquisitionMethod { get; set; } = string.Empty;
}

public class PurchaseSkinRequest
{
    public int SkinId { get; set; }
}

public class PurchaseResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int RemainingCoins { get; set; }

    public UserSkinDto? PurchasedSkin { get; set; }
}

public class EquipSkinRequest
{
    public int UserSkinId { get; set; }
}

public class EquipSkinResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int EquippedSkinId { get; set; }
}

public class UserInventoryDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public int Coins { get; set; }

    public List<UserSkinDto> Skins { get; set; } = new();

    public UserSkinDto? EquippedPlatformSkin { get; set; }

    public UserSkinDto? EquippedBallSkin { get; set; }
}
