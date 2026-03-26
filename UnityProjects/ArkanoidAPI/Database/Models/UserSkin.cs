namespace ArkanoidAPI.Models;

/// <summary>
/// Связь пользователя со скином (инвентарь)
/// </summary>
public class UserSkin
{
    public int Id { get; set; }

    /// <summary>
    /// ID пользователя
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID скина
    /// </summary>
    public int SkinId { get; set; }

    /// <summary>
    /// Дата получения скина
    /// </summary>
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Способ получения (покупка, награда, подарок)
    /// </summary>
    public AcquisitionMethod AcquisitionMethod { get; set; } = AcquisitionMethod.Purchase;

    /// <summary>
    /// Установлен ли скин как активный
    /// </summary>
    public bool IsEquipped { get; set; } = false;

    // Навигационные свойства
    public User? User { get; set; }
    public Skin? Skin { get; set; }
}

/// <summary>
/// Способ получения скина
/// </summary>
public enum AcquisitionMethod
{
    /// <summary>
    /// Покупка в магазине
    /// </summary>
    Purchase = 0,

    /// <summary>
    /// Награда за достижение
    /// </summary>
    Reward = 1,

    /// <summary>
    /// Подарок
    /// </summary>
    Gift = 2,

    /// <summary>
    /// Стартовый скин
    /// </summary>
    Starter = 3
}
