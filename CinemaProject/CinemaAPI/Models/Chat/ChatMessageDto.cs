namespace CinemaAPI.Models.Chat;

public class ChatMessageDto
{
    public int Id { get; set; }
    
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    
    public int? ReceiverId { get; set; }
    public string? ReceiverName { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }
}
