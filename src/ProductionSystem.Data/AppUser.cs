namespace ProductionSystem.Data;

public class AppUser
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public byte[]? Photo { get; set; }
}
