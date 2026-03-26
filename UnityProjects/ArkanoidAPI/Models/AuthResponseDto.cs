namespace ArkanoidAPI.Models;

/// <summary>
/// DTO для ответа при авторизации/регистрации
/// </summary>
public class AuthResponseDto
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
    /// JWT токен
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Время истечения токена
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
