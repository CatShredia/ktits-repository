namespace ArkanoidAPI.Models;

/// <summary>
/// История покупок скинов
/// </summary>
public class Purchase
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
    /// Цена покупки (в монетах)
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Дата покупки
    /// </summary>
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public User? User { get; set; }
    public Skin? Skin { get; set; }
}
