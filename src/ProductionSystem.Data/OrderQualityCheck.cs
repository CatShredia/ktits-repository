namespace ProductionSystem.Data;

/// <summary>Параметр контроля качества по заказу (+ / -).</summary>
public class OrderQualityCheck
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    /// <summary>+ или -</summary>
    public string Grade { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime CheckedAt { get; set; }
    public string CheckedByLogin { get; set; } = string.Empty;

    public CustomerOrder Order { get; set; } = null!;
}
