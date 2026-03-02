namespace TestApi3K.Database.Requests;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class CreateNewUserAndLogin
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public int id_Role { get; set; }
}

public class EditUserAndLogin
{
    public int id_User { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public int id_Role { get; set; }
}