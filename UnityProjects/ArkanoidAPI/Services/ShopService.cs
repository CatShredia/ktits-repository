
using ArkanoidAPI.Database;
using ArkanoidAPI.Database.DTOs;
using ArkanoidAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArkanoidAPI.Services;

public interface IShopService
{
    Task<IEnumerable<SkinDto>> GetAllSkinsAsync();

    Task<SkinDto?> GetSkinByIdAsync(int id);

    Task<UserInventoryDto> GetInventoryAsync(int userId);

    Task<IEnumerable<UserSkinDto>> GetUserSkinsAsync(int userId);

    Task<PurchaseResponse> PurchaseSkinAsync(int userId, int skinId);

    Task<EquipSkinResponse> EquipSkinAsync(int userId, int userSkinId);

    Task<EquipSkinResponse> UnequipSkinAsync(int userId, int userSkinId);

    Task<Dictionary<string, UserSkinDto>> GetEquippedSkinsAsync(int userId);
}

public class ShopService : IShopService
{
    private readonly ArkanoidDbContext _context;

    public ShopService(ArkanoidDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SkinDto>> GetAllSkinsAsync()
    {
        var skins = await _context.Skins
            .Where(s => s.IsActive)
            .ToListAsync();

        return skins.Select(s => new SkinDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            SkinType = s.SkinType.ToString(),
            Rarity = s.Rarity.ToString(),
            Price = s.Price,
            TexturePath = s.TexturePath,
            PrefabPath = s.PrefabPath,
            IsStarter = s.IsStarter,
            IsActive = s.IsActive
        });
    }

    public async Task<SkinDto?> GetSkinByIdAsync(int id)
    {
        var skin = await _context.Skins.FindAsync(id);
        if (skin == null)
        {
            return null;
        }

        return new SkinDto
        {
            Id = skin.Id,
            Name = skin.Name,
            Description = skin.Description,
            SkinType = skin.SkinType.ToString(),
            Rarity = skin.Rarity.ToString(),
            Price = skin.Price,
            TexturePath = skin.TexturePath,
            PrefabPath = skin.PrefabPath,
            IsStarter = skin.IsStarter,
            IsActive = skin.IsActive
        };
    }

    public async Task<UserInventoryDto> GetInventoryAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.UserSkins)
                .ThenInclude(us => us.Skin)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("Пользователь не найден");
        }

        var userSkins = user.UserSkins.Select(us => new UserSkinDto
        {
            Id = us.Id,
            SkinId = us.SkinId,
            SkinName = us.Skin?.Name ?? "Unknown",
            SkinType = us.Skin?.SkinType.ToString() ?? "Unknown",
            AcquiredAt = us.AcquiredAt,
            IsEquipped = us.IsEquipped,
            AcquisitionMethod = us.AcquisitionMethod.ToString()
        }).ToList();

        var equippedPlatform = userSkins.FirstOrDefault(us => us.IsEquipped && us.SkinType == SkinType.Platform.ToString());
        var equippedBall = userSkins.FirstOrDefault(us => us.IsEquipped && us.SkinType == SkinType.Ball.ToString());

        return new UserInventoryDto
        {
            UserId = user.Id,
            Username = user.Username,
            Coins = user.Coins,
            Skins = userSkins,
            EquippedPlatformSkin = equippedPlatform,
            EquippedBallSkin = equippedBall
        };
    }

    public async Task<IEnumerable<UserSkinDto>> GetUserSkinsAsync(int userId)
    {
        var userSkins = await _context.UserSkins
            .Include(us => us.Skin)
            .Where(us => us.UserId == userId)
            .ToListAsync();

        return userSkins.Select(us => new UserSkinDto
        {
            Id = us.Id,
            SkinId = us.SkinId,
            SkinName = us.Skin?.Name ?? "Unknown",
            SkinType = us.Skin?.SkinType.ToString() ?? "Unknown",
            AcquiredAt = us.AcquiredAt,
            IsEquipped = us.IsEquipped,
            AcquisitionMethod = us.AcquisitionMethod.ToString()
        });
    }

    public async Task<PurchaseResponse> PurchaseSkinAsync(int userId, int skinId)
    {
        var user = await _context.Users
            .Include(u => u.UserSkins)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return new PurchaseResponse
            {
                Success = false,
                Message = "Пользователь не найден",
                ErrorCode = PurchaseErrorCode.UserNotFound
            };
        }

        var skin = await _context.Skins.FindAsync(skinId);
        if (skin == null)
        {
            return new PurchaseResponse
            {
                Success = false,
                Message = "Скин не найден",
                ErrorCode = PurchaseErrorCode.SkinNotFound
            };
        }

        if (!skin.IsActive)
        {
            return new PurchaseResponse
            {
                Success = false,
                Message = "Скин недоступен для покупки",
                ErrorCode = PurchaseErrorCode.SkinNotAvailable
            };
        }

        // Проверка: уже есть ли этот скин у пользователя
        var existingUserSkin = user.UserSkins.FirstOrDefault(us => us.SkinId == skinId);
        if (existingUserSkin != null)
        {
            return new PurchaseResponse
            {
                Success = false,
                Message = "Этот скин уже есть у пользователя",
                ErrorCode = PurchaseErrorCode.AlreadyOwned
            };
        }

        // Проверка: достаточно ли монет
        if (user.Coins < skin.Price)
        {
            return new PurchaseResponse
            {
                Success = false,
                Message = $"Недостаточно монет. Требуется: {skin.Price}, Доступно: {user.Coins}",
                ErrorCode = PurchaseErrorCode.InsufficientCoins
            };
        }

        // Покупка
        user.Coins -= skin.Price;

        var userSkin = new UserSkin
        {
            UserId = userId,
            SkinId = skinId,
            AcquiredAt = DateTime.UtcNow,
            AcquisitionMethod = AcquisitionMethod.Purchase,
            IsEquipped = false
        };

        _context.UserSkins.Add(userSkin);

        // Создание записи о покупке
        var purchase = new Purchase
        {
            UserId = userId,
            SkinId = skinId,
            Price = skin.Price,
            PurchasedAt = DateTime.UtcNow
        };

        _context.Purchases.Add(purchase);

        await _context.SaveChangesAsync();

        return new PurchaseResponse
        {
            Success = true,
            Message = "Скин успешно куплен",
            RemainingCoins = user.Coins,
            PurchasedSkin = new UserSkinDto
            {
                Id = userSkin.Id,
                SkinId = userSkin.SkinId,
                SkinName = skin.Name,
                SkinType = skin.SkinType.ToString(),
                AcquiredAt = userSkin.AcquiredAt,
                IsEquipped = userSkin.IsEquipped,
                AcquisitionMethod = userSkin.AcquisitionMethod.ToString()
            }
        };
    }

    public async Task<EquipSkinResponse> EquipSkinAsync(int userId, int userSkinId)
    {
        var userSkin = await _context.UserSkins
            .Include(us => us.Skin)
            .FirstOrDefaultAsync(us => us.Id == userSkinId && us.UserId == userId);

        if (userSkin == null)
        {
            return new EquipSkinResponse
            {
                Success = false,
                Message = "Скин не найден или не принадлежит пользователю",
                ErrorCode = EquipErrorCode.SkinNotFound
            };
        }

        if (userSkin.Skin == null)
        {
            return new EquipSkinResponse
            {
                Success = false,
                Message = "Данные скина не найдены",
                ErrorCode = EquipErrorCode.SkinDataNotFound
            };
        }

        // Проверка: уже экипирован ли этот скин
        if (userSkin.IsEquipped)
        {
            return new EquipSkinResponse
            {
                Success = false,
                Message = "Этот скин уже экипирован",
                ErrorCode = EquipErrorCode.AlreadyEquipped
            };
        }

        // Снять скин того же типа
        var sameTypeSkins = await _context.UserSkins
            .Include(us => us.Skin)
            .Where(us => us.UserId == userId && us.IsEquipped)
            .ToListAsync();

        foreach (var existingEquipped in sameTypeSkins)
        {
            if (existingEquipped.Skin?.SkinType == userSkin.Skin?.SkinType)
            {
                existingEquipped.IsEquipped = false;
            }
        }

        // Надеть новый скин
        userSkin.IsEquipped = true;
        await _context.SaveChangesAsync();

        return new EquipSkinResponse
        {
            Success = true,
            Message = "Скин экипирован",
            EquippedSkinId = userSkin.SkinId
        };
    }

    public async Task<EquipSkinResponse> UnequipSkinAsync(int userId, int userSkinId)
    {
        var userSkin = await _context.UserSkins
            .Include(us => us.Skin)
            .FirstOrDefaultAsync(us => us.Id == userSkinId && us.UserId == userId);

        if (userSkin == null)
        {
            return new EquipSkinResponse
            {
                Success = false,
                Message = "Скин не найден или не принадлежит пользователю",
                ErrorCode = EquipErrorCode.SkinNotFound
            };
        }

        if (!userSkin.IsEquipped)
        {
            return new EquipSkinResponse
            {
                Success = false,
                Message = "Скин уже не экипирован",
                ErrorCode = EquipErrorCode.AlreadyEquipped
            };
        }

        // Снять скин
        userSkin.IsEquipped = false;
        await _context.SaveChangesAsync();

        return new EquipSkinResponse
        {
            Success = true,
            Message = "Скин снят",
            EquippedSkinId = userSkin.SkinId
        };
    }

    public async Task<Dictionary<string, UserSkinDto>> GetEquippedSkinsAsync(int userId)
    {
        var equippedSkins = await _context.UserSkins
            .Include(us => us.Skin)
            .Where(us => us.UserId == userId && us.IsEquipped)
            .ToListAsync();

        var result = new Dictionary<string, UserSkinDto>();

        foreach (var us in equippedSkins)
        {
            if (us.Skin != null)
            {
                result[us.Skin.SkinType.ToString()] = new UserSkinDto
                {
                    Id = us.Id,
                    SkinId = us.SkinId,
                    SkinName = us.Skin.Name,
                    SkinType = us.Skin.SkinType.ToString(),
                    AcquiredAt = us.AcquiredAt,
                    IsEquipped = us.IsEquipped,
                    AcquisitionMethod = us.AcquisitionMethod.ToString()
                };
            }
        }

        return result;
    }
}
