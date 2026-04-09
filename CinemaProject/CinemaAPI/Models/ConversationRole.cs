using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models;

public class ConversationRole
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
}
