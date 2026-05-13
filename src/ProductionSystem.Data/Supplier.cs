namespace ProductionSystem.Data;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int DeliveryDays { get; set; }

    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<StockComponent> Components { get; set; } = new List<StockComponent>();
}
