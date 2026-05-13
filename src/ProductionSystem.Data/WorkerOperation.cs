namespace ProductionSystem.Data;

public class WorkerOperation
{
    public int WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;
    public int OperationId { get; set; }
    public ProductionOperation Operation { get; set; } = null!;
}
