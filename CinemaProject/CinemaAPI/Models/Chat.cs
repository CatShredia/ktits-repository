namespace CinemaAPI.Models;

public class Chat
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsGeneral { get; set; }

    public int? User1Id { get; set; }
    public User? User1 { get; set; }

    public int? User2Id { get; set; }
    public User? User2 { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
