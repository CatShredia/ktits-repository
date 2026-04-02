namespace CinemaAPI.Models.Chat;

public class ConversationDto
{
    public int Id { get; set; }
    public ConversationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserChatDto> Participants { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
}
