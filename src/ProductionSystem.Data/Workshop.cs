namespace ProductionSystem.Data;

/// <summary>Цех производства.</summary>
public class Workshop
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte[]? FloorPlanImage { get; set; }

    public ICollection<WorkshopLayoutItem> LayoutItems { get; set; } = new List<WorkshopLayoutItem>();
}
