namespace ProductionSystem.Data;

/// <summary>Спецификация сборочные единицы (родительское изделие — дочернее изделие/деталь).</summary>
public class ProductAssemblySpec
{
    public string ParentProductName { get; set; } = string.Empty;
    public string ChildProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    public Product ParentProduct { get; set; } = null!;
    public Product ChildProduct { get; set; } = null!;
}
