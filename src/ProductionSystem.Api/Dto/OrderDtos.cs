namespace ProductionSystem.Api.Dto;

public class OrderListItemDto
{
    public string Number { get; set; } = "";
    public DateOnly OrderDate { get; set; }
    public string OrderName { get; set; } = "";
    public string Status { get; set; } = "";
    public decimal? EstimatedCost { get; set; }
    public string CustomerLogin { get; set; } = "";
    public string? CustomerName { get; set; }
    public DateOnly? PlannedCompletionDate { get; set; }
    public string? ManagerLogin { get; set; }
    public string? ManagerName { get; set; }
}

public class OrderDetailDto : OrderListItemDto
{
    public string ProductName { get; set; } = "";
    public string ProductDescription { get; set; } = "";
    public string? RejectionReason { get; set; }
    public List<OrderDimensionDto> Dimensions { get; set; } = new();
    public bool HasDrawings { get; set; }
}

public class OrderDimensionDto
{
    public int? Id { get; set; }
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Value { get; set; }
}

public class OrderCreateRequest
{
    public string OrderName { get; set; } = "";
    public string? CustomerLogin { get; set; }
    public string ProductDescription { get; set; } = "";
    public List<OrderDimensionDto> Dimensions { get; set; } = new();
    public string? DrawingsBase64 { get; set; }
}

public class OrderStatusChangeRequest
{
    public string Status { get; set; } = "";
    public string? Comment { get; set; }
    public decimal? EstimatedCost { get; set; }
    public DateOnly? PlannedCompletionDate { get; set; }
}

public class OrderStatusHistoryDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "";
    public DateTime ChangedAt { get; set; }
    public string? ChangedByLogin { get; set; }
    public string? Comment { get; set; }
}

public class WorkshopDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? FloorPlanBase64 { get; set; }
    public List<WorkshopLayoutItemDto> LayoutItems { get; set; } = new();
}

public class WorkshopLayoutItemDto
{
    public int? Id { get; set; }
    public string IconType { get; set; } = "";
    public double X { get; set; }
    public double Y { get; set; }
}

public class WorkshopLayoutSaveRequest
{
    public List<WorkshopLayoutItemDto> Items { get; set; } = new();
}

public class EquipmentFailureDto
{
    public int Id { get; set; }
    public string EquipmentMarking { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Reason { get; set; } = "";
    public string RegisteredByLogin { get; set; } = "";
}

public class EquipmentFailureCreateRequest
{
    public string EquipmentMarking { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public string Reason { get; set; } = "";
}

public class EquipmentFailureEndRequest
{
    public DateTime EndedAt { get; set; }
}

public class QualityCheckDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string ParameterName { get; set; } = "";
    public string Grade { get; set; } = "";
    public string? Comment { get; set; }
    public DateTime CheckedAt { get; set; }
}

public class QualityCheckUpsertRequest
{
    public string ParameterName { get; set; } = "";
    public string Grade { get; set; } = "";
    public string? Comment { get; set; }
}
