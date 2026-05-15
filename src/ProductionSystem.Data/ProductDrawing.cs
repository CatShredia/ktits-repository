namespace ProductionSystem.Data;

/// <summary>Чертёж изделия (от заказчика или конструктора).</summary>
public class ProductDrawing
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    /// <summary>Заказчик / Конструктор.</summary>
    public string Source { get; set; } = string.Empty;
    public byte[]? Content { get; set; }

    public Product Product { get; set; } = null!;
}
