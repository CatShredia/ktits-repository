namespace CinemaAPI.Models;

public class ChatMessage
{
    public int Id { get; set; }

    public int SenderId { get; set; }
    public User? Sender { get; set; }

    public int ChatId { get; set; }
    public Chat? Chat { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }
}
