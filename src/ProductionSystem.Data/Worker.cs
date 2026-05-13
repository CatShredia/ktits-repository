namespace ProductionSystem.Data;

public class Worker
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstMiddleName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string HomeAddress { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;

    public ICollection<WorkerOperation> WorkerOperations { get; set; } = new List<WorkerOperation>();
}
