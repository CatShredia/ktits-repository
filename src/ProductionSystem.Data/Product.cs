namespace ProductionSystem.Data;

/// <summary>Изделие (PK — наименование по словарю данных).</summary>
public class Product
{
    public string Name { get; set; } = string.Empty;
    public string Dimensions { get; set; } = string.Empty;

    public ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();
    public ICollection<ProductMaterialSpec> MaterialSpecs { get; set; } = new List<ProductMaterialSpec>();
    public ICollection<ProductComponentSpec> ComponentSpecs { get; set; } = new List<ProductComponentSpec>();
    public ICollection<ProductOperationSpec> OperationSpecs { get; set; } = new List<ProductOperationSpec>();
    /// <summary>Строки спецификации, где это изделие — родитель (состоит из дочерних).</summary>
    public ICollection<ProductAssemblySpec> AssemblyChildren { get; set; } = new List<ProductAssemblySpec>();
    /// <summary>Строки спецификации, где это изделие входит в состав другого.</summary>
    public ICollection<ProductAssemblySpec> AssemblyParents { get; set; } = new List<ProductAssemblySpec>();
    public ICollection<ProductDrawing> Drawings { get; set; } = new List<ProductDrawing>();
    public ICollection<ProductMeasurement> Measurements { get; set; } = new List<ProductMeasurement>();
}
