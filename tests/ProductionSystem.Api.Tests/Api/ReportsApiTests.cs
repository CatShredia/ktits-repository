using System.Net;
using ProductionSystem.Api.Tests.Infrastructure;

namespace ProductionSystem.Api.Tests.Api;

public class ReportsApiTests : ApiTestBase
{
    [Fact]
    public async Task InventoryMaterials_AsManager_ReturnsWarehouseGroups()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.ManagerLogin,
            TestDataSeeder.ManagerPassword);

        var response = await client.GetAsync("/api/reports/inventory?kind=materials");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var report = await response.Content.ReadFromJsonAsync<InventoryReportDto>();
        Assert.NotNull(report);
        Assert.Equal("Материалы", report!.Kind);
        Assert.NotEmpty(report.Warehouses);
        Assert.True(report.Warehouses[0].WarehouseTotalQuantity > 0);
    }

    [Fact]
    public async Task Inventory_InvalidKind_ReturnsBadRequest()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.DirectorLogin,
            TestDataSeeder.DirectorPassword);

        var response = await client.GetAsync("/api/reports/inventory?kind=unknown");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

internal sealed class InventoryReportDto
{
    public string Kind { get; set; } = "";
    public List<InventoryWarehouseDto> Warehouses { get; set; } = new();
}

internal sealed class InventoryWarehouseDto
{
    public string WarehouseName { get; set; } = "";
    public decimal WarehouseTotalQuantity { get; set; }
}
