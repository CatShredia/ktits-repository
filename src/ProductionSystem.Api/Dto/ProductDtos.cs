namespace ProductionSystem.Api.Dto;

public class ProductListItemDto
{
    public string Name { get; set; } = "";
    public string Dimensions { get; set; } = "";
    public int MaterialCount { get; set; }
    public int ComponentCount { get; set; }
    public int AssemblyCount { get; set; }
    public int OperationCount { get; set; }
}

public class ProductDetailDto
{
    public string Name { get; set; } = "";
    public string Dimensions { get; set; } = "";
    public List<ProductDrawingDto> Drawings { get; set; } = [];
    public List<ProductMeasurementDto> Measurements { get; set; } = [];
    public List<ProductMaterialLineDto> Materials { get; set; } = [];
    public List<ProductComponentLineDto> Components { get; set; } = [];
    public List<ProductAssemblyLineDto> Assemblies { get; set; } = [];
    public List<ProductOperationLineDto> Operations { get; set; } = [];
}

public class ProductDrawingDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Source { get; set; } = "";
    public bool HasContent { get; set; }
}

public class ProductMeasurementDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Value { get; set; }
}

public class ProductMaterialLineDto
{
    public int MaterialId { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class ProductComponentLineDto
{
    public int ComponentId { get; set; }
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class ProductAssemblyLineDto
{
    public string ChildProductName { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class ProductOperationLineDto
{
    public int OperationId { get; set; }
    public string OperationName { get; set; } = "";
    public int SequenceNumber { get; set; }
    public string? EquipmentTypeName { get; set; }
    public int DurationMinutes { get; set; }
    public string? Description { get; set; }
    public bool RequiresEquipment { get; set; }
}

public class ProductUpdateRequest
{
    public string Dimensions { get; set; } = "";
    public List<ProductMeasurementDto> Measurements { get; set; } = [];
    public List<ProductMaterialLineDto> Materials { get; set; } = [];
    public List<ProductComponentLineDto> Components { get; set; } = [];
    public List<ProductAssemblyLineDto> Assemblies { get; set; } = [];
    public List<ProductOperationLineDto> Operations { get; set; } = [];
}

public class ProductDrawingCreateRequest
{
    public string Title { get; set; } = "";
    public string Source { get; set; } = "Конструктор";
    public string? ContentBase64 { get; set; }
}
