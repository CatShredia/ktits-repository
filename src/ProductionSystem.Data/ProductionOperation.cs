namespace ProductionSystem.Data;

public class ProductionOperation
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<WorkerOperation> WorkerOperations { get; set; } = new List<WorkerOperation>();
    public ICollection<ProductOperationSpec> ProductOperationSpecs { get; set; } = new List<ProductOperationSpec>();
}
