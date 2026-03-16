namespace CinemaAPI.Models;

public class Film
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int? GenreId { get; set; }
    public Genre? Genre { get; set; }
    public string? ImageUrl { get; set; }
}
