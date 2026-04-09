namespace CinemaAPI.Data.Models.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int SenderId { get; set; }
    public UserBriefDto Sender { get; set; } = null!;
}

public class CommentCreateDto
{
    public string Content { get; set; } = string.Empty;
}
