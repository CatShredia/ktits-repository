namespace TestApi3K.Requests;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}