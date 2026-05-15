namespace ProductionSystem.Client.Models;

public class OperationsCatalogDto
{
    public List<OperationCatalogItem> Operations { get; set; } = new();
    public List<string> EquipmentTypes { get; set; } = new();
}

public class OperationCatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
