namespace ProductionSystem.Api.Dto;

public class OrderPlanningDto
{
    public string OrderNumber { get; set; } = "";
    public string ProductName { get; set; } = "";
    public List<ProcurementLineDto> ProcurementLines { get; set; } = [];
    public decimal TotalProcurementCost { get; set; }
    public int MinDeliveryDays { get; set; }
    public int MinDeliveryDaysForShortage { get; set; }
    public int ProductionMinutes { get; set; }
    public int TotalMinutes { get; set; }
    public List<GanttBarDto> GanttBars { get; set; } = [];
    public List<string> EquipmentUsed { get; set; } = [];
}

public class ProcurementLineDto
{
    public string Kind { get; set; } = "";
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ShortageQuantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal LineCost { get; set; }
    public int? DeliveryDays { get; set; }
}

public class GanttBarDto
{
    public string ProductName { get; set; } = "";
    public string OperationName { get; set; } = "";
    public string? EquipmentTypeName { get; set; }
    public string? EquipmentMarking { get; set; }
    public int StartMinutes { get; set; }
    public int EndMinutes { get; set; }
    public bool IsBackground { get; set; }
}
