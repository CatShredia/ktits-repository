namespace CinemaAPI.Models.ChatModels;

public class ChatMessageDto
{
    public int Id { get; set; }

    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;

    public int ChatId { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }
}
