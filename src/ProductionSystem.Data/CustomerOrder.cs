namespace ProductionSystem.Data;

/// <summary>Заказ (PK — номер заказа).</summary>
public class CustomerOrder
{
    public string Number { get; set; } = string.Empty;
    public string OrderName { get; set; } = string.Empty;
    public DateOnly OrderDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CustomerLogin { get; set; } = string.Empty;
    public string? ManagerLogin { get; set; }
    public decimal? EstimatedCost { get; set; }
    public DateOnly? PlannedCompletionDate { get; set; }
    public byte[]? CustomerDrawings { get; set; }

    public Product Product { get; set; } = null!;
    public AppUser Customer { get; set; } = null!;
    public AppUser? Manager { get; set; }
}
