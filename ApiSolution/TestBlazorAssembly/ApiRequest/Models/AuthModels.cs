namespace TestBlazorAssembly.ApiRequest.Models;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
}