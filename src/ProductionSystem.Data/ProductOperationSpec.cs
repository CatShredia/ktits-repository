namespace ProductionSystem.Data;

/// <summary>Спецификация операции (изделие, операция, порядковый номер, тип оборудования, время).</summary>
public class ProductOperationSpec
{
    public string ProductName { get; set; } = string.Empty;
    public int OperationId { get; set; }
    public int SequenceNumber { get; set; }
    public string? EquipmentTypeName { get; set; }
    /// <summary>Длительность операции (в минутах).</summary>
    public int DurationMinutes { get; set; }

    public Product Product { get; set; } = null!;
    public ProductionOperation Operation { get; set; } = null!;
    public EquipmentType? EquipmentType { get; set; }
}
