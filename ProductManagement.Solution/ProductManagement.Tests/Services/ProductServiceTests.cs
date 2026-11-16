using ProductManagement;

namespace ProductManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly ProductService _service = new();

    private static Product Valid() => new()
    {
        Id = 1, Name = "Ноутбук", Price = 50000m, Quantity = 3, Category = "Электроника"
    };

    // ---- Задание 1: CalculateTotalPrice ----

    [Theory]
    [InlineData(100, 2, 200)]
    [InlineData(250, 4, 1000)]
    [InlineData(50, 0, 0)]
    [InlineData(125.5, 2, 251)]
    public void CalculateTotalPrice_ReturnsPriceTimesQuantity(double price, int quantity, double expected)
    {
        var result = _service.CalculateTotalPrice((decimal)price, quantity);
        Assert.Equal((decimal)expected, result);
    }

    [Fact]
    public void CalculateTotalPrice_NegativePrice_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateTotalPrice(-1m, 2));

    [Fact]
    public void CalculateTotalPrice_NegativeQuantity_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateTotalPrice(10m, -1));

    // ---- Задание 2: CalculateDiscount ----

    [Theory]
    [InlineData(1000, 10, 900)]
    [InlineData(500, 20, 400)]
    [InlineData(100, 0, 100)]
    [InlineData(2000, 50, 1000)]
    [InlineData(100, 100, 0)]
    public void CalculateDiscount_ReturnsPriceWithDiscount(double total, double percent, double expected)
    {
        var result = _service.CalculateDiscount((decimal)total, (decimal)percent);
        Assert.Equal((decimal)expected, result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    [InlineData(150)]
    public void CalculateDiscount_InvalidPercent_Throws(double percent)
        => Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateDiscount(1000m, (decimal)percent));

    [Fact]
    public void CalculateDiscount_NegativeTotal_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateDiscount(-1m, 10m));

    // ---- Задание 3: ValidateProduct ----

    [Fact]
    public void ValidateProduct_CorrectProduct_ReturnsTrue()
        => Assert.True(_service.ValidateProduct(Valid()));

    [Fact]
    public void ValidateProduct_ZeroQuantity_ReturnsTrue()
    {
        var p = Valid(); p.Quantity = 0;
        Assert.True(_service.ValidateProduct(p));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateProduct_EmptyName_ReturnsFalse(string name)
    {
        var p = Valid(); p.Name = name;
        Assert.False(_service.ValidateProduct(p));
    }

    [Fact]
    public void ValidateProduct_ZeroPrice_ReturnsFalse()
    {
        var p = Valid(); p.Price = 0;
        Assert.False(_service.ValidateProduct(p));
    }

    [Fact]
    public void ValidateProduct_NegativePrice_ReturnsFalse()
    {
        var p = Valid(); p.Price = -10;
        Assert.False(_service.ValidateProduct(p));
    }

    [Fact]
    public void ValidateProduct_NegativeQuantity_ReturnsFalse()
    {
        var p = Valid(); p.Quantity = -1;
        Assert.False(_service.ValidateProduct(p));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateProduct_EmptyCategory_ReturnsFalse(string category)
    {
        var p = Valid(); p.Category = category;
        Assert.False(_service.ValidateProduct(p));
    }

    [Fact]
    public void ValidateProduct_Null_Throws()
        => Assert.Throws<ArgumentNullException>(() => _service.ValidateProduct(null!));
}
