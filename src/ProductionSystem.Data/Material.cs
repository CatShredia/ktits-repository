namespace ProductionSystem.Data;

public class Material
{
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public byte[]? Image { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public string? Gost { get; set; }
    public decimal? Length { get; set; }
    public string? Characteristics { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}
