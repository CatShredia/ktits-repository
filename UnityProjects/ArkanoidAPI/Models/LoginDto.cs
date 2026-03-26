using System.ComponentModel.DataAnnotations;

namespace ArkanoidAPI.Models;

/// <summary>
/// DTO для входа пользователя
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Пароль
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}
