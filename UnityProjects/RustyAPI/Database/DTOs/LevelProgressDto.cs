using System.ComponentModel.DataAnnotations;

namespace RustyAPI.Database.DTOs;

public class LevelProgressDto
{
    [Required]
    public string LevelKey { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int LevelIndex { get; set; }

    [Range(0, 3)]
    public int StarsCollected { get; set; }

    public bool Completed { get; set; }
}
