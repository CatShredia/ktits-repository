using Npgsql;

namespace AutoService.Data.Workshop;

// Список отзывов и сохранение нового.
// Вызывается из ReviewsViewModel.
public sealed class ReviewService
{
    private readonly IDbContextFactory<AutoServiceDbContext> _factory;

    public ReviewService(IDbContextFactory<AutoServiceDbContext> factory) => _factory = factory;

    public async Task<IReadOnlyList<ReviewRow>> GetReviewsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var rows = await db.Reviews.AsNoTracking()
            .Include(x => x.Client)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return rows.Select(x => new ReviewRow
        {
            Id = x.Id,
            WorkOrderId = x.WorkOrderId,
            CreatedAt = x.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            ClientName = x.Client.FullName,
            Rating = x.Rating,
            Comment = x.Comment
        }).ToList();
    }

    public async Task<IReadOnlyList<LookupItem>> GetOrdersWithoutReviewAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        var orders = await db.WorkOrders.AsNoTracking()
            .Include(x => x.Appointment).ThenInclude(x => x.Client)
            .Include(x => x.Appointment).ThenInclude(x => x.Car)
            .Where(x => x.Status == WorkOrderStatus.Завершён && x.Review == null)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        return orders.Select(x => new LookupItem
        {
            Id = x.Id,
            Title = $"№{x.Id} · {x.Appointment.Client.FullName} · {x.Appointment.Car.LicensePlate}"
        }).ToList();
    }

    public async Task SaveReviewAsync(int workOrderId, int rating, string? comment, DateTime createdAt)
    {
        var text = comment?.Trim() ?? "";
        if (text.Length == 0)
            throw new WorkshopException("Заполните поле «Комментарий».");
        if (text.Length > FieldLimits.Comment)
            throw new WorkshopException($"Поле «Комментарий» длиннее {FieldLimits.Comment} символов.");
        if (rating is < 1 or > 5)
            throw new WorkshopException("Оценка должна быть от 1 до 5.");

        await using var db = await _factory.CreateDbContextAsync();
        var order = await db.WorkOrders.Include(x => x.Appointment).FirstOrDefaultAsync(x => x.Id == workOrderId)
            ?? throw new WorkshopException("Заказ-наряд не найден.");
        if (order.Status != WorkOrderStatus.Завершён)
            throw new WorkshopException("Отзыв можно оставить только по завершённому заказ-наряду.");
        if (await db.Reviews.AnyAsync(x => x.WorkOrderId == workOrderId))
            throw new WorkshopException("По этому заказ-наряду отзыв уже есть.");

        db.Reviews.Add(new Review
        {
            ClientId = order.Appointment.ClientId,
            WorkOrderId = workOrderId,
            Rating = rating,
            Comment = text,
            CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Unspecified)
        });
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (FindPostgres(ex)?.SqlState == "23505")
        {
            throw new WorkshopException("По этому заказ-наряду отзыв уже есть.");
        }
    }

    private static PostgresException? FindPostgres(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is PostgresException postgres)
                return postgres;
        }

        return null;
    }
}
