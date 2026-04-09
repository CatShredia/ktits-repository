using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Data.Models;

public class Conversation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ConversationTypeId { get; set; }

    public ConversationType ConversationType { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? ParentMessageId { get; set; }
    public Message? ParentMessage { get; set; }

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
