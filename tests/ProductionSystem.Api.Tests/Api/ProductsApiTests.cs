using System.Net;
using ProductionSystem.Api.Tests.Infrastructure;

namespace ProductionSystem.Api.Tests.Api;

public class ProductsApiTests : ApiTestBase
{
    [Fact]
    public async Task GetProducts_AsForeman_ReturnsSeededProduct()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.ForemanLogin,
            TestDataSeeder.ForemanPassword);

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();
        Assert.NotNull(list);
        Assert.Contains(list!, p => p.Name == "Изделие тестовое");
    }

    [Fact]
    public async Task GetProduct_AsForeman_ReturnsSpecification()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.ForemanLogin,
            TestDataSeeder.ForemanPassword);

        var response = await client.GetAsync("/api/products/Изделие%20тестовое");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = await response.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(detail);
        Assert.NotEmpty(detail!.Materials);
    }
}

internal sealed class ProductListItemDto
{
    public string Name { get; set; } = "";
}

internal sealed class ProductDetailDto
{
    public string Name { get; set; } = "";
    public List<MaterialLineDto> Materials { get; set; } = new();
}

internal sealed class MaterialLineDto
{
    public decimal Quantity { get; set; }
}
