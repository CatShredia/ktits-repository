namespace ProductionSystem.Data;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<StockComponent> Components { get; set; } = new List<StockComponent>();
}
