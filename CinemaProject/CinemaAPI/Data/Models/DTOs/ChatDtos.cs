using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Data.Models.DTOs;

public class CreateConversationDto
{
    [Required]
    public List<int> ParticipantIds { get; set; } = new();

    [Required]
    public string ConversationTypeName { get; set; } = "Direct";
}

public class AddParticipantDto
{
    [Required]
    public int UserId { get; set; }

    public string? RoleName { get; set; }
}

public class SendMessageDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message content must be between 1 and 1000 characters")]
    public string Content { get; set; } = string.Empty;
}

public class SendMessageWithImageDto
{
    [StringLength(1000, ErrorMessage = "Message content must not exceed 1000 characters")]
    public string? Content { get; set; }
}

public class EditMessageDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message content must be between 1 and 1000 characters")]
    public string Content { get; set; } = string.Empty;
}
