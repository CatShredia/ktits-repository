namespace CinemaAPI.Models.DTOs;

public class RatingCreateDto
{
    public int Value { get; set; }
    public int FilmId { get; set; }
}

public class RatingUpdateDto
{
    public int Value { get; set; }
}

public class RatingResponseDto
{
    public int Id { get; set; }
    public int Value { get; set; }
    public int FilmId { get; set; }
    public string? FilmName { get; set; }
    public int? AuthorId { get; set; }
    public string? AuthorName { get; set; }
}
