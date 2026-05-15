using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Services;

public class OrderWorkflowService(AppDbContext db, MaterialWriteOffService writeOff)
{
    public async Task<(bool Ok, string? Error)> TransitionAsync(
        CustomerOrder order,
        string newStatus,
        string? login,
        string? comment,
        CancellationToken ct)
    {
        if (!OrderWorkflowService.CanTransition(order.Status, newStatus))
            return (false, "Недопустимый переход статуса.");

        if (newStatus == OrderStatuses.Production && order.Status == OrderStatuses.Procurement)
        {
            var (ok, err) = await writeOff.WriteOffForOrderAsync(order.Number, ct);
            if (!ok)
                return (false, err);
        }

        order.Status = newStatus;
        if (!string.IsNullOrWhiteSpace(comment) && newStatus is OrderStatuses.Cancelled or OrderStatuses.Rejected)
            order.RejectionReason = comment;

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            OrderNumber = order.Number,
            Status = newStatus,
            ChangedAt = DateTime.UtcNow,
            ChangedByLogin = login,
            Comment = comment,
        });

        await db.SaveChangesAsync(ct);
        return (true, null);
    }

    public static bool CanTransition(string from, string to) =>
        from switch
        {
            OrderStatuses.New when to is OrderStatuses.Cancelled or OrderStatuses.Specification => true,
            OrderStatuses.Specification when to is OrderStatuses.Confirmation => true,
            OrderStatuses.Confirmation when to is OrderStatuses.Rejected or OrderStatuses.Cancelled or OrderStatuses.Procurement => true,
            OrderStatuses.Procurement when to is OrderStatuses.Production => true,
            OrderStatuses.Production when to is OrderStatuses.QualityControl => true,
            OrderStatuses.QualityControl when to is OrderStatuses.Ready => true,
            OrderStatuses.Ready when to is OrderStatuses.Closed => true,
            _ => false,
        };
}
