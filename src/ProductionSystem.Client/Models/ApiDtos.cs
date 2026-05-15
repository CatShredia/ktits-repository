namespace ProductionSystem.Client.Models;

public class AuthResponse
{
    public string Token { get; set; } = "";
    public string Login { get; set; } = "";
    public string Role { get; set; } = "";
    public string? FullName { get; set; }
}

public class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class MaterialDto
{
    public int Id { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public string? SupplierName { get; set; }
    public int? SupplierDeliveryDays { get; set; }
    public string MaterialType { get; set; } = "";
    public decimal PurchasePrice { get; set; }
    public string? Gost { get; set; }
    public decimal? Length { get; set; }
    public string? Characteristics { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
}

public class MaterialListResponse
{
    public List<MaterialDto> Items { get; set; } = new();
    public int FilteredPositionCount { get; set; }
    public decimal FilteredTotalQuantity { get; set; }
    public decimal FilteredTotalPurchaseCost { get; set; }
    public int TotalPositionsInDatabase { get; set; }
}

public class MaterialUpdateRequest
{
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public string MaterialType { get; set; } = "";
    public decimal PurchasePrice { get; set; }
    public string? Gost { get; set; }
    public decimal? Length { get; set; }
    public string? Characteristics { get; set; }
    public int WarehouseId { get; set; }
}

public class ComponentDto
{
    public int Id { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public string? SupplierName { get; set; }
    public int? SupplierDeliveryDays { get; set; }
    public string ComponentType { get; set; } = "";
    public decimal PurchasePrice { get; set; }
    public decimal Weight { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
}

public class ComponentListResponse
{
    public List<ComponentDto> Items { get; set; } = new();
    public int FilteredPositionCount { get; set; }
    public decimal FilteredTotalQuantity { get; set; }
    public decimal FilteredTotalPurchaseCost { get; set; }
    public int TotalPositionsInDatabase { get; set; }
}

public class ComponentUpdateRequest
{
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public string ComponentType { get; set; } = "";
    public decimal PurchasePrice { get; set; }
    public decimal Weight { get; set; }
    public int WarehouseId { get; set; }
}

public class WorkerListItemDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    public string OperationsCommaSeparated { get; set; } = "";
}

public class WorkerDetailDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = "";
    public string FirstMiddleName { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string HomeAddress { get; set; } = "";
    public string Education { get; set; } = "";
    public string Qualification { get; set; } = "";
    public List<int> OperationIds { get; set; } = new();
}

public class WorkerCreateUpdateRequest
{
    public string LastName { get; set; } = "";
    public string FirstMiddleName { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string HomeAddress { get; set; } = "";
    public string Education { get; set; } = "";
    public string Qualification { get; set; } = "";
    public List<int> OperationIds { get; set; } = new();
}

public class ProductionOperationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

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

public class EquipmentFailureDto
{
    public int Id { get; set; }
    public string EquipmentMarking { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Reason { get; set; } = "";
    public string RegisteredByLogin { get; set; } = "";
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

public class CustomerUserDto
{
    public string Login { get; set; } = "";
    public string? FullName { get; set; }
}

public class EquipmentListItemDto
{
    public string Marking { get; set; } = "";
    public string EquipmentTypeName { get; set; } = "";
    public string? Characteristics { get; set; }
}

public class ProductListItemDto
{
    public string Name { get; set; } = "";
    public string Dimensions { get; set; } = "";
    public int MaterialCount { get; set; }
    public int ComponentCount { get; set; }
    public int AssemblyCount { get; set; }
    public int OperationCount { get; set; }
}

public class ProductDetailDto
{
    public string Name { get; set; } = "";
    public string Dimensions { get; set; } = "";
    public List<ProductDrawingDto> Drawings { get; set; } = new();
    public List<ProductMeasurementDto> Measurements { get; set; } = new();
    public List<ProductMaterialLineDto> Materials { get; set; } = new();
    public List<ProductComponentLineDto> Components { get; set; } = new();
    public List<ProductAssemblyLineDto> Assemblies { get; set; } = new();
    public List<ProductOperationLineDto> Operations { get; set; } = new();
}

public class ProductDrawingDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Source { get; set; } = "";
    public bool HasContent { get; set; }
}

public class ProductMeasurementDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Value { get; set; }
}

public class ProductMaterialLineDto
{
    public int MaterialId { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class ProductComponentLineDto
{
    public int ComponentId { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class ProductAssemblyLineDto
{
    public string ChildProductName { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class ProductOperationLineDto
{
    public int OperationId { get; set; }
    public string OperationName { get; set; } = "";
    public int SequenceNumber { get; set; }
    public string? EquipmentTypeName { get; set; }
    public int DurationMinutes { get; set; }
    public string? Description { get; set; }
    public bool RequiresEquipment { get; set; } = true;
}

public class ProductUpdateRequest
{
    public string Dimensions { get; set; } = "";
    public List<ProductMeasurementDto> Measurements { get; set; } = new();
    public List<ProductMaterialLineDto> Materials { get; set; } = new();
    public List<ProductComponentLineDto> Components { get; set; } = new();
    public List<ProductAssemblyLineDto> Assemblies { get; set; } = new();
    public List<ProductOperationLineDto> Operations { get; set; } = new();
}

public class OrderPlanningDto
{
    public string OrderNumber { get; set; } = "";
    public string ProductName { get; set; } = "";
    public List<ProcurementLineDto> ProcurementLines { get; set; } = new();
    public decimal TotalProcurementCost { get; set; }
    public int MinDeliveryDays { get; set; }
    public int MinDeliveryDaysForShortage { get; set; }
    public int ProductionMinutes { get; set; }
    public int TotalMinutes { get; set; }
    public List<GanttBarDto> GanttBars { get; set; } = new();
    public List<string> EquipmentUsed { get; set; } = new();
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

public class InventoryReportResponse
{
    public string Kind { get; set; } = "";
    public string? TypeFilter { get; set; }
    public List<InventoryWarehouseGroupDto> Warehouses { get; set; } = new();
    public decimal GrandTotalQuantity { get; set; }
}

public class InventoryWarehouseGroupDto
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = "";
    public List<InventoryReportLineDto> Lines { get; set; } = new();
    public decimal WarehouseTotalQuantity { get; set; }
}

public class InventoryReportLineDto
{
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
}
