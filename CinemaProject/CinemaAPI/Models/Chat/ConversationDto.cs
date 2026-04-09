namespace CinemaAPI.Models.Chat;

public class ConversationDto
{
    public int Id { get; set; }
    public string ConversationTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
}
