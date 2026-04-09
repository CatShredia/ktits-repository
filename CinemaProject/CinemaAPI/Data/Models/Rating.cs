namespace CinemaAPI.Data.Models;

public class Rating
{
    public int Id { get; set; }
    public int Value { get; set; }
    public int FilmId { get; set; }
    public Film? Film { get; set; }
    public int? AuthorId { get; set; }
    public User? Author { get; set; }
}
