using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models;

public class Message
{
    [Key]
    public int Id { get; set; }
        
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    [Required]
    public string Content { get; set; } = string.Empty;
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}