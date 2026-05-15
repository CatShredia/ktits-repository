using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionSystem.Api.Dto;
using ProductionSystem.Api.Services;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(
    AppDbContext db,
    OrderWorkflowService workflow) : ControllerBase
{
    [HttpGet("customers")]
    [Authorize(Roles = UserRoles.Manager)]
    public async Task<ActionResult<List<object>>> Customers(CancellationToken ct)
    {
        var list = await db.Users.AsNoTracking()
            .Where(u => u.Role == UserRoles.Customer)
            .OrderBy(u => u.FullName)
            .Select(u => new { u.Login, u.FullName })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderListItemDto>>> List(
        [FromQuery] string? filter,
        CancellationToken ct)
    {
        var login = AuthUserAccessor.GetLogin(User)!;
        var role = AuthUserAccessor.GetRole(User)!;

        var query = db.CustomerOrders.AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Manager)
            .AsQueryable();

        query = role switch
        {
            UserRoles.Customer => query.Where(o => o.CustomerLogin == login),
            UserRoles.Manager => query.Where(o =>
                o.Status == OrderStatuses.New || o.ManagerLogin == login),
            UserRoles.Designer => query.Where(o => o.Status == OrderStatuses.Specification),
            UserRoles.Foreman => query.Where(o =>
                o.Status == OrderStatuses.Production || o.Status == OrderStatuses.QualityControl),
            _ => query,
        };

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var statuses = filter.ToLowerInvariant() switch
            {
                "new" => new[] { OrderStatuses.New, OrderStatuses.Specification, OrderStatuses.Confirmation },
                "current" => new[] { OrderStatuses.Procurement, OrderStatuses.Production, OrderStatuses.QualityControl },
                "completed" => new[] { OrderStatuses.Ready, OrderStatuses.Closed },
                "rejected" => new[] { OrderStatuses.Rejected, OrderStatuses.Cancelled },
                _ => Array.Empty<string>(),
            };
            if (statuses.Length > 0)
                query = query.Where(o => statuses.Contains(o.Status));
        }

        var list = await query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Number).ToListAsync(ct);
        return Ok(list.Select(MapList).ToList());
    }

    [HttpGet("{number}")]
    public async Task<ActionResult<OrderDetailDto>> Get(string number, CancellationToken ct)
    {
        var order = await LoadOrderAsync(number, ct);
        if (order is null)
            return NotFound();
        if (!CanView(order))
            return Forbid();

        return Ok(await MapDetailAsync(order, ct));
    }

    [HttpGet("{number}/history")]
    [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Director}")]
    public async Task<ActionResult<List<OrderStatusHistoryDto>>> History(string number, CancellationToken ct)
    {
        var exists = await db.CustomerOrders.AnyAsync(o => o.Number == number, ct);
        if (!exists)
            return NotFound();

        var items = await db.OrderStatusHistory.AsNoTracking()
            .Where(h => h.OrderNumber == number)
            .OrderBy(h => h.ChangedAt)
            .Select(h => new OrderStatusHistoryDto
            {
                Id = h.Id,
                Status = h.Status,
                ChangedAt = h.ChangedAt,
                ChangedByLogin = h.ChangedByLogin,
                Comment = h.Comment,
            })
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Manager}")]
    public async Task<ActionResult<OrderDetailDto>> Create([FromBody] OrderCreateRequest req, CancellationToken ct)
    {
        var login = AuthUserAccessor.GetLogin(User)!;
        var role = AuthUserAccessor.GetRole(User)!;

        if (string.IsNullOrWhiteSpace(req.OrderName))
            return BadRequest(new { message = "Укажите наименование заказа." });

        var customerLogin = role == UserRoles.Customer
            ? login
            : req.CustomerLogin?.Trim();

        if (string.IsNullOrWhiteSpace(customerLogin))
            return BadRequest(new { message = "Укажите заказчика." });

        if (!await db.Users.AnyAsync(u => u.Login == customerLogin && u.Role == UserRoles.Customer, ct))
            return BadRequest(new { message = "Заказчик не найден." });

        var orderDate = DateOnly.FromDateTime(DateTime.Today);
        var number = await OrderNumberGenerator.GenerateAsync(db, customerLogin, orderDate, ct);

        var productName = $"Заказ-{number}";
        if (!await db.Products.AnyAsync(p => p.Name == productName, ct))
        {
            db.Products.Add(new Product
            {
                Name = productName,
                Dimensions = FormatDimensions(req.Dimensions),
            });
        }

        var status = role == UserRoles.Manager ? OrderStatuses.Specification : OrderStatuses.New;
        var order = new CustomerOrder
        {
            Number = number,
            OrderName = req.OrderName.Trim(),
            OrderDate = orderDate,
            ProductName = productName,
            ProductDescription = req.ProductDescription?.Trim() ?? "",
            CustomerLogin = customerLogin,
            ManagerLogin = role == UserRoles.Manager ? login : null,
            Status = status,
            CustomerDrawings = DecodeDrawings(req.DrawingsBase64),
        };

        foreach (var d in req.Dimensions)
        {
            order.Dimensions.Add(new OrderDimension
            {
                Description = d.Description.Trim(),
                Unit = d.Unit.Trim(),
                Value = d.Value,
            });
        }

        db.CustomerOrders.Add(order);
        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            OrderNumber = number,
            Status = status,
            ChangedAt = DateTime.UtcNow,
            ChangedByLogin = login,
        });

        await db.SaveChangesAsync(ct);

        var created = await LoadOrderAsync(number, ct);
        return CreatedAtAction(nameof(Get), new { number }, await MapDetailAsync(created!, ct));
    }

    [HttpPut("{number}")]
    [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Manager}")]
    public async Task<IActionResult> Update(string number, [FromBody] OrderCreateRequest req, CancellationToken ct)
    {
        var order = await db.CustomerOrders
            .Include(o => o.Dimensions)
            .FirstOrDefaultAsync(o => o.Number == number, ct);
        if (order is null)
            return NotFound();
        if (!OrderStatuses.CanEditOrder(order.Status))
            return BadRequest(new { message = "Редактирование возможно только для статуса «Новый»." });
        if (!CanEdit(order))
            return Forbid();

        order.OrderName = req.OrderName.Trim();
        order.ProductDescription = req.ProductDescription?.Trim() ?? "";
        if (req.DrawingsBase64 is not null)
            order.CustomerDrawings = DecodeDrawings(req.DrawingsBase64);

        db.OrderDimensions.RemoveRange(order.Dimensions);
        order.Dimensions.Clear();
        foreach (var d in req.Dimensions)
        {
            order.Dimensions.Add(new OrderDimension
            {
                Description = d.Description.Trim(),
                Unit = d.Unit.Trim(),
                Value = d.Value,
            });
        }

        var product = await db.Products.FirstAsync(p => p.Name == order.ProductName, ct);
        product.Dimensions = FormatDimensions(req.Dimensions);

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{number}")]
    [Authorize(Roles = $"{UserRoles.Customer},{UserRoles.Manager}")]
    public async Task<IActionResult> Delete(string number, CancellationToken ct)
    {
        var order = await db.CustomerOrders.FirstOrDefaultAsync(o => o.Number == number, ct);
        if (order is null)
            return NotFound();
        if (!OrderStatuses.CanEditOrder(order.Status))
            return BadRequest(new { message = "Удаление возможно только для статуса «Новый»." });
        if (!CanEdit(order))
            return Forbid();

        db.CustomerOrders.Remove(order);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{number}/status")]
    public async Task<IActionResult> ChangeStatus(
        string number,
        [FromBody] OrderStatusChangeRequest req,
        CancellationToken ct)
    {
        var order = await db.CustomerOrders
            .Include(o => o.QualityChecks)
            .FirstOrDefaultAsync(o => o.Number == number, ct);
        if (order is null)
            return NotFound();

        var login = AuthUserAccessor.GetLogin(User)!;
        var role = AuthUserAccessor.GetRole(User)!;
        var target = req.Status.Trim();

        if (role == UserRoles.Director)
            return Forbid();

        var (allowed, err) = ValidateRoleTransition(order, target, role, login, req);
        if (!allowed)
            return BadRequest(new { message = err });

        if (target == OrderStatuses.Ready && role == UserRoles.Foreman)
        {
            if (order.QualityChecks.Count == 0 ||
                order.QualityChecks.Any(q => q.Grade != "+"))
                return BadRequest(new { message = "Все параметры контроля качества должны иметь оценку «+»." });
        }

        if (target == OrderStatuses.Confirmation && role == UserRoles.Designer)
        {
            if (req.EstimatedCost is null || req.PlannedCompletionDate is null)
                return BadRequest(new { message = "Укажите стоимость и плановую дату завершения." });
            order.EstimatedCost = req.EstimatedCost;
            order.PlannedCompletionDate = req.PlannedCompletionDate;
        }

        if (target == OrderStatuses.Specification && role == UserRoles.Manager && order.Status == OrderStatuses.New)
            order.ManagerLogin = login;

        var (ok, wfErr) = await workflow.TransitionAsync(order, target, login, req.Comment, ct);
        if (!ok)
            return BadRequest(new { message = wfErr });

        return NoContent();
    }

    [HttpPost("{number}/cancel")]
    [Authorize(Roles = UserRoles.Customer)]
    public async Task<IActionResult> CancelByCustomer(string number, [FromBody] OrderStatusChangeRequest? req, CancellationToken ct)
    {
        var order = await db.CustomerOrders.FirstOrDefaultAsync(o => o.Number == number, ct);
        if (order is null)
            return NotFound();
        if (order.CustomerLogin != AuthUserAccessor.GetLogin(User))
            return Forbid();

        var allowed = order.Status is OrderStatuses.New or OrderStatuses.Specification
            or OrderStatuses.Confirmation or OrderStatuses.Rejected;
        if (!allowed)
            return BadRequest(new { message = "Отмена недоступна после этапа «Закупка»." });

        var (ok, err) = await workflow.TransitionAsync(
            order, OrderStatuses.Cancelled, AuthUserAccessor.GetLogin(User), req?.Comment, ct);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    private static (bool Allowed, string? Error) ValidateRoleTransition(
        CustomerOrder order,
        string target,
        string role,
        string login,
        OrderStatusChangeRequest req)
    {
        if (!OrderWorkflowService.CanTransition(order.Status, target))
            return (false, "Недопустимый переход статуса.");

        return (role, order.Status, target) switch
        {
            (UserRoles.Manager, OrderStatuses.New, OrderStatuses.Specification) => (true, null),
            (UserRoles.Manager, OrderStatuses.New, OrderStatuses.Cancelled) => (true, null),
            (UserRoles.Manager, OrderStatuses.Confirmation, OrderStatuses.Rejected) => (true, null),
            (UserRoles.Manager, OrderStatuses.Confirmation, OrderStatuses.Procurement) => (true, null),
            (UserRoles.Manager, OrderStatuses.Procurement, OrderStatuses.Production) => (true, null),
            (UserRoles.Manager, OrderStatuses.Ready, OrderStatuses.Closed) => (true, null),
            (UserRoles.Designer, OrderStatuses.Specification, OrderStatuses.Confirmation) => (true, null),
            (UserRoles.Foreman, OrderStatuses.Production, OrderStatuses.QualityControl) => (true, null),
            (UserRoles.Foreman, OrderStatuses.QualityControl, OrderStatuses.Ready) => (true, null),
            _ => (false, "Недостаточно прав для смены статуса."),
        };
    }

    private bool CanView(CustomerOrder order)
    {
        var login = AuthUserAccessor.GetLogin(User)!;
        var role = AuthUserAccessor.GetRole(User)!;
        return role switch
        {
            UserRoles.Customer => order.CustomerLogin == login,
            UserRoles.Manager => order.Status == OrderStatuses.New || order.ManagerLogin == login,
            UserRoles.Designer => order.Status == OrderStatuses.Specification,
            UserRoles.Foreman => order.Status is OrderStatuses.Production or OrderStatuses.QualityControl,
            UserRoles.Director => true,
            _ => false,
        };
    }

    private bool CanEdit(CustomerOrder order)
    {
        var login = AuthUserAccessor.GetLogin(User)!;
        var role = AuthUserAccessor.GetRole(User)!;
        return role switch
        {
            UserRoles.Customer => order.CustomerLogin == login,
            UserRoles.Manager => true,
            _ => false,
        };
    }

    private async Task<CustomerOrder?> LoadOrderAsync(string number, CancellationToken ct) =>
        await db.CustomerOrders.AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Manager)
            .Include(o => o.Dimensions)
            .FirstOrDefaultAsync(o => o.Number == number, ct);

    private static OrderListItemDto MapList(CustomerOrder o) => new()
    {
        Number = o.Number,
        OrderDate = o.OrderDate,
        OrderName = o.OrderName,
        Status = o.Status,
        EstimatedCost = o.EstimatedCost,
        CustomerLogin = o.CustomerLogin,
        CustomerName = o.Customer?.FullName,
        PlannedCompletionDate = o.PlannedCompletionDate,
        ManagerLogin = o.ManagerLogin,
        ManagerName = o.Manager?.FullName,
    };

    private static async Task<OrderDetailDto> MapDetailAsync(CustomerOrder o, CancellationToken ct) => new()
    {
        Number = o.Number,
        OrderDate = o.OrderDate,
        OrderName = o.OrderName,
        Status = o.Status,
        EstimatedCost = o.EstimatedCost,
        CustomerLogin = o.CustomerLogin,
        CustomerName = o.Customer?.FullName,
        PlannedCompletionDate = o.PlannedCompletionDate,
        ManagerLogin = o.ManagerLogin,
        ManagerName = o.Manager?.FullName,
        ProductName = o.ProductName,
        ProductDescription = o.ProductDescription,
        RejectionReason = o.RejectionReason,
        HasDrawings = o.CustomerDrawings is { Length: > 0 },
        Dimensions = o.Dimensions.Select(d => new OrderDimensionDto
        {
            Id = d.Id,
            Description = d.Description,
            Unit = d.Unit,
            Value = d.Value,
        }).ToList(),
    };

    private static string FormatDimensions(IEnumerable<OrderDimensionDto> dims) =>
        string.Join("; ", dims.Select(d => $"{d.Description}: {d.Value} {d.Unit}"));

    private static byte[]? DecodeDrawings(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return null;
        try
        {
            var data = base64.Contains(',') ? base64[(base64.IndexOf(',') + 1)..] : base64;
            return Convert.FromBase64String(data);
        }
        catch
        {
            return null;
        }
    }
}
