namespace ProductionSystem.Data;

/// <summary>Комплектующее на складе (имя сущности без конфликта с System.Component).</summary>
public class StockComponent
{
    public int Id { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public byte[]? Image { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal Weight { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
