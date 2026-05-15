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
    public string Status { get; set; } = OrderStatuses.New;
    public string? RejectionReason { get; set; }
    /// <summary>Описание изделия (текст заявки).</summary>
    public string ProductDescription { get; set; } = string.Empty;

    public Product Product { get; set; } = null!;
    public AppUser Customer { get; set; } = null!;
    public AppUser? Manager { get; set; }
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<OrderDimension> Dimensions { get; set; } = new List<OrderDimension>();
    public ICollection<OrderQualityCheck> QualityChecks { get; set; } = new List<OrderQualityCheck>();
}
