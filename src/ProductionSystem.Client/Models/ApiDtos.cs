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
