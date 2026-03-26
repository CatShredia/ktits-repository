using System.ComponentModel.DataAnnotations;

namespace ArkanoidAPI.Models;

/// <summary>
/// DTO для регистрации пользователя
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Имя пользователя (3-50 символов)
    /// </summary>
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    [MinLength(3, ErrorMessage = "Имя должно содержать минимум 3 символа")]
    [MaxLength(50, ErrorMessage = "Имя должно содержать максимум 50 символов")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Пароль (минимум 6 символов)
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; } = string.Empty;
}
