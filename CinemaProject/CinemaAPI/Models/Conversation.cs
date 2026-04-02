using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models;

public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public ConversationType Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public enum ConversationType
{
    Direct,
    Group
}
