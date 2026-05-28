using ProductionSystem.Api.Services;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Tests.Unit;

public class OrderWorkflowServiceTests
{
    [Theory]
    [InlineData(OrderStatuses.New, OrderStatuses.Specification, true)]
    [InlineData(OrderStatuses.New, OrderStatuses.Cancelled, true)]
    [InlineData(OrderStatuses.Procurement, OrderStatuses.Production, true)]
    [InlineData(OrderStatuses.New, OrderStatuses.Closed, false)]
    [InlineData(OrderStatuses.Ready, OrderStatuses.Production, false)]
    public void CanTransition_ReturnsExpected(string from, string to, bool expected)
    {
        var actual = OrderWorkflowService.CanTransition(from, to);

        Assert.Equal(expected, actual);
    }
}
