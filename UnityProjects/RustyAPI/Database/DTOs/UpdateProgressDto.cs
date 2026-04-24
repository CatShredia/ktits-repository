using System.ComponentModel.DataAnnotations;

namespace RustyAPI.Database.DTOs;

public class UpdateProgressDto
{
    [Range(0, int.MaxValue)]
    public int LastCompletedLevelIndex { get; set; }

    public List<LevelProgressDto> LevelProgresses { get; set; } = new();
}
