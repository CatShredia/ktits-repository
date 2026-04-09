namespace CinemaAPI.Data.Models;


public class ConversationParticipant
{
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public ConversationRole Role { get; set; } = null!;
}