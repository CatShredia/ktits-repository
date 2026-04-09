namespace CinemaAPI.Data.Models.DTOs;

public class ConversationDto
{
    public int Id { get; set; }
    public string ConversationTypeName { get; set; } = string.Empty;
    public int? ParentMessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
}

public class CommentPreviewDto
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
