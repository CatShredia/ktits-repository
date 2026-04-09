namespace CinemaAPI.Data.Models.DTOs;

public class LoginInfoDto
{
    public int Id { get; set; }
    public string LoginValue { get; set; } = string.Empty;
}

public class LoginUpdateSimpleDto
{
    public string? LoginValue { get; set; }
    public string? Password { get; set; }
}

public class LoginCreateSimpleDto
{
    public string LoginValue { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

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
    public string? Password { get; set; }
}
