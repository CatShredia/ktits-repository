namespace CinemaAPI.Models.DTOs;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UserResponseDto
{
    public int Id { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
}

// DTO для отображения пользователя с логином (для списка)
public class UserListDto
{
    public int Id { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? LoginValue { get; set; }
}

// DTO для создания пользователя
public class UserCreateDto
{
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? RoleId { get; set; }
}

// DTO для обновления пользователя
public class UserUpdateDto
{
    public int Id { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? RoleId { get; set; }
}

// DTO для создания/обновления логина
public class LoginCreateDto
{
    public string LoginValue { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int UserId { get; set; }
}

public class LoginUpdateDto
{
    public int Id { get; set; }
    public string LoginValue { get; set; } = string.Empty;
    public string? Password { get; set; } // Если null, пароль не меняется
}

public class UserRegisterDto
{
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? RoleId { get; set; } // Для регистрации админом
}

public class UserLoginDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserResponseDto User { get; set; } = new();
    public string Role { get; set; } = string.Empty;
}
