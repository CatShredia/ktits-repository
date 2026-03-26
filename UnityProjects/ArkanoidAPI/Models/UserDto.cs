namespace ArkanoidAPI.Models;

/// <summary>
/// DTO для отображения информации о пользователе
/// </summary>
public class UserDto
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Уникальный GUID пользователя
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Баланс монет
    /// </summary>
    public int Coins { get; set; }

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего входа
    /// </summary>
    public DateTime LastLoginAt { get; set; }
}
