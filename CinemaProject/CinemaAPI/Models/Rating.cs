namespace CinemaAPI.Models;

public class Rating
{
    public int Id { get; set; }
    public int Value { get; set; }
    public int FilmId { get; set; }
    public Film? Film { get; set; }
}
