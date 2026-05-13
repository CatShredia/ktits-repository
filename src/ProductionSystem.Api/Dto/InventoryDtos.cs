namespace ProductionSystem.Api.Dto;

public class MaterialDto
{
    public int Id { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? SupplierName { get; set; }
    public int? SupplierDeliveryDays { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public string? Gost { get; set; }
    public decimal? Length { get; set; }
    public string? Characteristics { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? ImageBase64 { get; set; }
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
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public string? Gost { get; set; }
    public decimal? Length { get; set; }
    public string? Characteristics { get; set; }
    public int WarehouseId { get; set; }
}

public class ComponentDto
{
    public int Id { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? SupplierName { get; set; }
    public int? SupplierDeliveryDays { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal Weight { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? ImageBase64 { get; set; }
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
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal Weight { get; set; }
    public int WarehouseId { get; set; }
}

public class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
