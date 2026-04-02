using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Chat;

public class SendMessageDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message content must be between 1 and 1000 characters")]
    public string Content { get; set; } = string.Empty;
}
