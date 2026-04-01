namespace CinemaAPI.Models.ChatModels;

public class ChatDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsGeneral { get; set; }

    public int? User1Id { get; set; }
    public string? User1Name { get; set; }

    public int? User2Id { get; set; }
    public string? User2Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UnreadCount { get; set; }

    public ChatMessageDto? LastMessage { get; set; }
}
