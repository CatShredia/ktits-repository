namespace ProductionSystem.Data;

/// <summary>Замер изделия (наименование, единица, значение).</summary>
public class ProductMeasurement
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Value { get; set; }

    public Product Product { get; set; } = null!;
}
