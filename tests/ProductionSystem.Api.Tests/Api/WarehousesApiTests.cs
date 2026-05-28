using System.Net;
using ProductionSystem.Api.Tests.Infrastructure;

namespace ProductionSystem.Api.Tests.Api;

public class WarehousesApiTests : ApiTestBase
{
    [Fact]
    public async Task GetWarehouses_WithoutToken_ReturnsUnauthorized()
    {
        using var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/warehouses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetWarehouses_AsManager_ReturnsSeededWarehouse()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.ManagerLogin,
            TestDataSeeder.ManagerPassword);

        var response = await client.GetAsync("/api/warehouses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<WarehouseDto>>();
        Assert.NotNull(list);
        Assert.Contains(list!, w => w.Name == "Склад №1");
    }
}

internal sealed class WarehouseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
