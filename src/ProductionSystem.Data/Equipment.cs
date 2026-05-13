namespace ProductionSystem.Data;

/// <summary>Оборудование (PK — маркировка).</summary>
public class Equipment
{
    public string Marking { get; set; } = string.Empty;
    public string EquipmentTypeName { get; set; } = string.Empty;
    public string? Characteristics { get; set; }

    public EquipmentType EquipmentType { get; set; } = null!;
}
