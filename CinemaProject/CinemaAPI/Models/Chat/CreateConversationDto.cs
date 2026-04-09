using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Models.Chat;

public class CreateConversationDto
{
    [Required]
    public List<int> ParticipantIds { get; set; } = new();

    [Required]
    public string ConversationTypeName { get; set; } = "Direct";
}
