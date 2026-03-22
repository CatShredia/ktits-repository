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
    public int? AuthorId { get; set; }
    public User? Author { get; set; }
    public ICollection<Rating>? Ratings { get; set; }
    
    // Вычисляемое поле для среднего рейтинга (не мапится в БД)
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public double AverageRating => Ratings?.Any() == true ? Ratings.Average(r => r.Value) : 0;
}
