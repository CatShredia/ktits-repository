using System.ComponentModel.DataAnnotations;

namespace CinemaAPI.Data.Models;

public class ConversationType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
