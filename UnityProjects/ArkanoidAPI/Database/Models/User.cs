namespace ArkanoidAPI.Models;

/// <summary>
/// Пользователь игры
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор пользователя (GUID для клиента)
    /// </summary>
    public string UserId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Хэш пароля
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Баланс игровой валюты (монеты)
    /// </summary>
    public int Coins { get; set; } = 100;

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего входа
    /// </summary>
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    /// <summary>
    /// Инвентарь скинов пользователя
    /// </summary>
    public ICollection<UserSkin> UserSkins { get; set; } = new List<UserSkin>();

    /// <summary>
    /// История покупок
    /// </summary>
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
