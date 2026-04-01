namespace CinemaAPI.Models.ChatModels;

public class OnlineUserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}
