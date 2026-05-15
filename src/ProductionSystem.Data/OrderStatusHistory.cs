namespace ProductionSystem.Data;

public class OrderStatusHistory
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? ChangedByLogin { get; set; }
    public string? Comment { get; set; }

    public CustomerOrder Order { get; set; } = null!;
}
