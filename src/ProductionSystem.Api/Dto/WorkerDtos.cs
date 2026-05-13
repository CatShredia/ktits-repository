namespace ProductionSystem.Api.Dto;

public class WorkerListItemDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string OperationsCommaSeparated { get; set; } = string.Empty;
}

public class WorkerDetailDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstMiddleName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string HomeAddress { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public List<int> OperationIds { get; set; } = new();
}

public class WorkerCreateUpdateRequest
{
    public string LastName { get; set; } = string.Empty;
    public string FirstMiddleName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string HomeAddress { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public List<int> OperationIds { get; set; } = new();
}

public class ProductionOperationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
