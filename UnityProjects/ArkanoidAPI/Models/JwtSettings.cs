namespace ArkanoidAPI.Models;

/// <summary>
/// Настройки JWT аутентификации
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Секретный ключ для подписи токенов (минимум 32 символа)
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Издатель токена
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Получатель токена
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Время жизни токена в минутах
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}
