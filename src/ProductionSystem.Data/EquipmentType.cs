namespace ProductionSystem.Data;

/// <summary>Тип оборудования (PK — наименование типа).</summary>
public class EquipmentType
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    public ICollection<ProductOperationSpec> OperationSpecs { get; set; } = new List<ProductOperationSpec>();
}
