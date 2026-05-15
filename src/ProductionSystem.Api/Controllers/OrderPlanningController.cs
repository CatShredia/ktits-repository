using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Api.Services;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/orders/{number}/planning")]
public class OrderPlanningController(
    AppDbContext db,
    ProcurementEstimationService procurement,
    ProductionSchedulingService scheduling) : ControllerBase
{
    private static readonly HashSet<string> AllowedStatuses =
    [
        OrderStatuses.Specification,
        OrderStatuses.Confirmation,
        OrderStatuses.Procurement,
        OrderStatuses.Production,
        OrderStatuses.QualityControl,
        OrderStatuses.Ready,
        OrderStatuses.Closed,
    ];

    [HttpGet]
    [Authorize(Roles = UserRoles.Manager)]
    public async Task<ActionResult<OrderPlanningDto>> Get(string number, CancellationToken ct)
    {
        var order = await db.CustomerOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Number == number, ct);
        if (order is null)
            return NotFound();

        if (!AllowedStatuses.Contains(order.Status))
            return BadRequest(new { message = "Оценка доступна после начала составления спецификации на заказ." });

        var hasSpecs = await db.ProductMaterialSpecs.AnyAsync(s => s.ProductName == order.ProductName, ct)
            || await db.ProductComponentSpecs.AnyAsync(s => s.ProductName == order.ProductName, ct)
            || await db.ProductOperationSpecs.AnyAsync(s => s.ProductName == order.ProductName, ct);
        if (!hasSpecs)
            return BadRequest(new { message = "Для изделия заказа не задана спецификация." });

        var proc = await procurement.EstimateAsync(order.ProductName, ct);
        var sched = await scheduling.ScheduleAsync(order.ProductName, ct);

        var productionMinutes = sched.ProductionMinutes;
        var deliveryMinutes = proc.MinDeliveryDaysForShortage * 24 * 60;

        return Ok(new OrderPlanningDto
        {
            OrderNumber = order.Number,
            ProductName = order.ProductName,
            ProcurementLines = proc.Lines.Select(l => new ProcurementLineDto
            {
                Kind = l.Kind,
                Article = l.Article,
                Name = l.Name,
                Unit = l.Unit,
                RequiredQuantity = l.RequiredQuantity,
                AvailableQuantity = l.AvailableQuantity,
                ShortageQuantity = l.ShortageQuantity,
                PurchasePrice = l.PurchasePrice,
                LineCost = l.LineCost,
                DeliveryDays = l.DeliveryDays,
            }).ToList(),
            TotalProcurementCost = proc.TotalProcurementCost,
            MinDeliveryDays = proc.MinDeliveryDays,
            MinDeliveryDaysForShortage = proc.MinDeliveryDaysForShortage,
            ProductionMinutes = productionMinutes,
            TotalMinutes = productionMinutes + deliveryMinutes,
            GanttBars = sched.Bars.Select(b => new GanttBarDto
            {
                ProductName = b.ProductName,
                OperationName = b.OperationName,
                EquipmentTypeName = b.EquipmentTypeName,
                EquipmentMarking = b.EquipmentMarking,
                StartMinutes = b.StartMinutes,
                EndMinutes = b.EndMinutes,
                IsBackground = b.IsBackground,
            }).ToList(),
            EquipmentUsed = sched.EquipmentUsed.ToList(),
        });
    }
}
