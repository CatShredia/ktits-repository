namespace ArkanoidAPI.Models;

/// <summary>
/// Тип скина
/// </summary>
public enum SkinType
{
    /// <summary>
    /// Скины для платформы
    /// </summary>
    Platform = 0,

    /// <summary>
    /// Скины для мяча
    /// </summary>
    Ball = 1
}

/// <summary>
/// Редкость скина
/// </summary>
public enum SkinRarity
{
    /// <summary>
    /// Обычный
    /// </summary>
    Common = 0,

    /// <summary>
    /// Необычный
    /// </summary>
    Uncommon = 1,

    /// <summary>
    /// Редкий
    /// </summary>
    Rare = 2,

    /// <summary>
    /// Эпический
    /// </summary>
    Epic = 3,

    /// <summary>
    /// Легендарный
    /// </summary>
    Legendary = 4
}

/// <summary>
/// Скип (внешний вид) для платформы или мяча
/// </summary>
public class Skin
{
    public int Id { get; set; }

    /// <summary>
    /// Уникальное имя скина
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание скина
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Тип скина (платформа/мяч)
    /// </summary>
    public SkinType SkinType { get; set; }

    /// <summary>
    /// Редкость скина
    /// </summary>
    public SkinRarity Rarity { get; set; } = SkinRarity.Common;

    /// <summary>
    /// Цена в монетах (0 для бесплатных/стартовых)
    /// </summary>
    public int Price { get; set; } = 0;

    /// <summary>
    /// Путь к текстуре/спрайту
    /// </summary>
    public string TexturePath { get; set; } = string.Empty;

    /// <summary>
    /// Путь к префабу (если используется уникальный префаб)
    /// </summary>
    public string? PrefabPath { get; set; }

    /// <summary>
    /// Является ли скин стартовым (доступен всем по умолчанию)
    /// </summary>
    public bool IsStarter { get; set; } = false;

    /// <summary>
    /// Активен ли скин в магазине
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Дата добавления в магазин
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    /// <summary>
    /// Пользователи, владеющие этим скином
    /// </summary>
    public ICollection<UserSkin> UserSkins { get; set; } = new List<UserSkin>();

    /// <summary>
    /// История покупок этого скина
    /// </summary>
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
