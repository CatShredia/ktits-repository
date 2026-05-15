namespace ProductionSystem.Api.Dto;

public class InventoryReportResponse
{
    public string Kind { get; set; } = "";
    public string? TypeFilter { get; set; }
    public List<InventoryWarehouseGroupDto> Warehouses { get; set; } = [];
    public decimal GrandTotalQuantity { get; set; }
}

public class InventoryWarehouseGroupDto
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = "";
    public List<InventoryReportLineDto> Lines { get; set; } = [];
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
