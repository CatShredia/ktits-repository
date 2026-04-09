namespace CinemaBlazor.Models;

public class Film
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int? GenreId { get; set; }
    public Genre? Genre { get; set; }
    public string? ImageUrl { get; set; }
    public int? AuthorId { get; set; }
    public UserBriefDto? Author { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int CommentsCount { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
}

public class UserBriefDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}
