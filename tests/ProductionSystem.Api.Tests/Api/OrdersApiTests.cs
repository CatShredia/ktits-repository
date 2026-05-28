using System.Net;
using System.Net.Http.Json;
using ProductionSystem.Api.Tests.Infrastructure;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Tests.Api;

public class OrdersApiTests : ApiTestBase
{
    [Fact]
    public async Task GetOrders_AsCustomer_ReturnsOnlyOwnOrders()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.CustomerLogin,
            TestDataSeeder.CustomerPassword);

        var response = await client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<OrderListItemDto>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list!);
        Assert.All(list!, o => Assert.Equal(TestDataSeeder.CustomerLogin, o.CustomerLogin));
    }

    [Fact]
    public async Task CreateOrder_AsCustomer_ReturnsCreated()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.CustomerLogin,
            TestDataSeeder.CustomerPassword);

        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            orderName = "Заказ из теста API",
            productDescription = "Описание изделия",
            dimensions = new[]
            {
                new { description = "Длина", unit = "м", value = 10m },
            },
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderListItemDto>();
        Assert.NotNull(order);
        Assert.Equal("Заказ из теста API", order!.OrderName);
        Assert.Equal(OrderStatuses.New, order.Status);
    }

    [Fact]
    public async Task GetOrderHistory_AsManager_ReturnsHistory()
    {
        using var client = Factory.CreateAuthenticatedClient(
            TestDataSeeder.ManagerLogin,
            TestDataSeeder.ManagerPassword);

        var response = await client.GetAsync("/api/orders/ЗК-TEST-0001/history");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var history = await response.Content.ReadFromJsonAsync<List<OrderHistoryDto>>();
        Assert.NotNull(history);
    }
}

internal sealed class OrderListItemDto
{
    public string Number { get; set; } = "";
    public string OrderName { get; set; } = "";
    public string Status { get; set; } = "";
    public string CustomerLogin { get; set; } = "";
}

internal sealed class OrderHistoryDto
{
    public string Status { get; set; } = "";
}
