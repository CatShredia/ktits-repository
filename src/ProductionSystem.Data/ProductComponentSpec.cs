namespace ProductionSystem.Data;

/// <summary>Спецификация комплектующие.</summary>
public class ProductComponentSpec
{
    public string ProductName { get; set; } = string.Empty;
    public int ComponentId { get; set; }
    public decimal Quantity { get; set; }

    public Product Product { get; set; } = null!;
    public StockComponent Component { get; set; } = null!;
}
