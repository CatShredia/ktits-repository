namespace CinemaAPI.Models.ChatModels;

public class SendMessageRequest
{
    public int ChatId { get; set; }
    public string Message { get; set; } = string.Empty;
}
