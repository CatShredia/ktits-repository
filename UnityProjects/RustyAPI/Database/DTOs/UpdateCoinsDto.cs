using System.ComponentModel.DataAnnotations;

namespace RustyAPI.Database.DTOs;

public class UpdateCoinsDto
{
    [Range(1, int.MaxValue)]
    public int CoinsDelta { get; set; }
}
