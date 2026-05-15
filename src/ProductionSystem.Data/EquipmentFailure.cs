namespace ProductionSystem.Data;

public class EquipmentFailure
{
    public int Id { get; set; }
    public string EquipmentMarking { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string RegisteredByLogin { get; set; } = string.Empty;

    public Equipment Equipment { get; set; } = null!;
    public AppUser RegisteredBy { get; set; } = null!;
}
