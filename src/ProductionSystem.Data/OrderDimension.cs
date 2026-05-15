namespace ProductionSystem.Data;

/// <summary>Замер размеров изделия в заказе.</summary>
public class OrderDimension
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Value { get; set; }

    public CustomerOrder Order { get; set; } = null!;
}
