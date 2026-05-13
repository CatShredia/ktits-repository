namespace ProductionSystem.Data;

/// <summary>Спецификация материалы (изделие — материал — количество).</summary>
public class ProductMaterialSpec
{
    public string ProductName { get; set; } = string.Empty;
    public int MaterialId { get; set; }
    public decimal Quantity { get; set; }

    public Product Product { get; set; } = null!;
    public Material Material { get; set; } = null!;
}
