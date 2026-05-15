namespace ProductionSystem.Data;

/// <summary>Значок на плане цеха (координаты 0..1 относительно размера плана).</summary>
public class WorkshopLayoutItem
{
    public int Id { get; set; }
    public int WorkshopId { get; set; }
    /// <summary>Equipment, FireExtinguisher, FirstAid, Exit</summary>
    public string IconType { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }

    public Workshop Workshop { get; set; } = null!;
}
