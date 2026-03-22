namespace CinemaAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Surname { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public Role? Role { get; set; }

    // Navigation property
    public Login? Login { get; set; }
    public ICollection<Film>? Films { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
}
