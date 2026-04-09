namespace CinemaAPI.Data.Models.DTOs;

public class FilmDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int? GenreId { get; set; }
    public GenreDto? Genre { get; set; }
    public string? ImageUrl { get; set; }
    public int? AuthorId { get; set; }
    public UserBriefDto? Author { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int CommentsCount { get; set; }
}

public class FilmCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int GenreId { get; set; }
    public string? ExternalImageUrl { get; set; }
}

public class FilmUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int GenreId { get; set; }
    public string? ExternalImageUrl { get; set; }
    public bool RemoveImage { get; set; }
}

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
