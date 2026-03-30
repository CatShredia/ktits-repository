namespace CinemaAPI.Models.Chat;

public class SendMessageRequest
{
    public string FromUser { get; set; } = string.Empty;
    public string ToUser { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
