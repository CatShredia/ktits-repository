namespace AutoService.Data.Workshop;

// Строка выпадающего списка: идентификатор и подпись.
// Используется в моделях экранов и сервисах Workshop.
public sealed class LookupItem
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public override string ToString() => Title;
}

// Страница списка и общее число строк.
// Возвращается AdminService и читается в PagedViewModel.
public sealed class PageResult<T>
{
    public PageResult(IReadOnlyList<T> items, int total, int page, int pageSize)
    {
        Items = items;
        Total = total;
        Page = page;
        PageSize = pageSize;
    }

    public IReadOnlyList<T> Items { get; }
    public int Total { get; }
    public int Page { get; }
    public int PageSize { get; }
}

// Сотрудник после успешного входа.
// Создаётся в AuthService и хранится в AppSession.
public sealed class CurrentUser
{
    public int Id { get; init; }
    public string FullName { get; init; } = "";
    public string Login { get; init; } = "";
    public string Role { get; init; } = "";
}

// Результат входа: пользователь или текст ошибки.
// Возвращается AuthService в LoginViewModel.
public sealed class AuthResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public CurrentUser? User { get; init; }
}

// Клиент в таблице администратора.
// Создаётся в AdminService и показывается в ClientsViewModel.
public sealed class ClientRow
{
    public int Id { get; init; }
    public string FullName { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Email { get; init; } = "";
}

// Автомобиль в таблице администратора.
// Создаётся в AdminService и показывается в CarsViewModel.
public sealed class CarRow
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public int BrandId { get; init; }
    public int CategoryId { get; init; }
    public string ClientName { get; init; } = "";
    public string Brand { get; init; } = "";
    public string Category { get; init; } = "";
    public string Model { get; init; } = "";
    public int Year { get; init; }
    public string Vin { get; init; } = "";
    public string LicensePlate { get; init; } = "";
    public int Mileage { get; init; }
    public string Color { get; init; } = "";
    public string Notes { get; init; } = "";
}

// Строка справочника.
// Создаётся в AdminService и показывается в CatalogViewModel.
public sealed class CatalogRow
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Email { get; init; } = "";
    public string Sku { get; init; } = "";
    public string Login { get; init; } = "";
    public decimal Price { get; init; }
    public decimal PurchasePrice { get; init; }
    public int Duration { get; init; }
    public int Quantity { get; init; }
    public int MinQuantity { get; init; }
    public int SupplierId { get; init; }
    public int DepartmentId { get; init; }
    public int SpecializationId { get; init; }
    public RepairBayKind Kind { get; init; }
    public int[] ServiceIds { get; init; } = [];

    public override string ToString() => Title;
}

public enum CatalogKind
{
    Brands,
    Categories,
    Departments,
    Specializations,
    Services,
    Suppliers,
    Parts,
    Bays,
    Mechanics
}

// Запись на обслуживание в списке.
// Создаётся в AdminService и показывается в AppointmentsViewModel.
public sealed class AppointmentRow
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public int CarId { get; init; }
    public int ServiceId { get; init; }
    public int MechanicId { get; init; }
    public int BayId { get; init; }
    public string When { get; init; } = "";
    public DateTime ScheduledAt { get; init; }
    public string ClientName { get; init; } = "";
    public string Car { get; init; } = "";
    public string Service { get; init; } = "";
    public string Mechanic { get; init; } = "";
    public string Bay { get; init; } = "";
    public string Status { get; init; } = "";
    public AppointmentStatus StatusValue { get; init; }
}

// Смена механика на ремонтном месте.
// Создаётся в AdminService и показывается в SchedulesViewModel.
public sealed class ScheduleRow
{
    public int Id { get; init; }
    public int MechanicId { get; init; }
    public int BayId { get; init; }
    public string Mechanic { get; init; } = "";
    public string Bay { get; init; } = "";
    public string Starts { get; init; } = "";
    public string Ends { get; init; } = "";
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
}

// Заказ-наряд в списке администратора.
// Создаётся в AdminService и показывается в WorkOrdersViewModel.
public sealed class WorkOrderRow
{
    public int Id { get; init; }
    public string CreatedAt { get; init; } = "";
    public string Status { get; init; } = "";
    public string Mechanic { get; init; } = "";
    public string ClientName { get; init; } = "";
    public string Car { get; init; } = "";
    public string Service { get; init; } = "";
}

// Услуга в постраничном списке справочника.
// Создаётся в AdminService и показывается в CatalogViewModel.
public sealed class ServiceRow
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal BasePrice { get; init; }
    public int DurationMinutes { get; init; }
}

// Запчасть в постраничном списке справочника.
// Создаётся в AdminService и показывается в CatalogViewModel.
public sealed class PartRow
{
    public int Id { get; init; }
    public int SupplierId { get; init; }
    public string Name { get; init; } = "";
    public string Sku { get; init; } = "";
    public string Supplier { get; init; } = "";
    public decimal PurchasePrice { get; init; }
    public decimal SalePrice { get; init; }
    public int Quantity { get; init; }
    public int MinQuantity { get; init; }
}

// Счёт с остатком оплаты.
// Создаётся в AdminService и показывается в InvoicesViewModel.
public sealed class InvoiceRow
{
    public int Id { get; init; }
    public int WorkOrderId { get; init; }
    public string CreatedAt { get; init; } = "";
    public string Status { get; init; } = "";
    public decimal AmountBeforeDiscount { get; init; }
    public decimal Discount { get; init; }
    public decimal TaxRate { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
    public decimal Paid { get; init; }
    public decimal Remainder { get; init; }
    public override string ToString() => $"Счёт №{Id}, остаток {Remainder:0.00} ₽";
}

// Запись механика без заказ-наряда.
// Создаётся в MechanicWorkService и показывается в MechanicOrdersViewModel.
public sealed class OpenAppointmentRow
{
    public int Id { get; init; }
    public string When { get; init; } = "";
    public string ClientName { get; init; } = "";
    public string Car { get; init; } = "";
    public string Service { get; init; } = "";
    public override string ToString() => $"{When} · {ClientName} · {Car} · {Service}";
}

// Заказ-наряд в списке механика.
// Создаётся в MechanicWorkService и показывается в MechanicOrdersViewModel.
public sealed class MechanicOrderRow
{
    public int Id { get; init; }
    public string CreatedAt { get; init; } = "";
    public string Status { get; init; } = "";
    public WorkOrderStatus StatusValue { get; init; }
    public string ClientName { get; init; } = "";
    public string Car { get; init; } = "";
    public string Service { get; init; } = "";
    public override string ToString() => $"№{Id} · {CreatedAt} · {Status} · {ClientName}";
}

// Услуга или запчасть в карточке заказа.
// Создаётся в MechanicWorkService и показывается в MechanicOrdersViewModel.
public sealed class OrderLineRow
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public override string ToString() => Title;
}

// Карточка заказ-наряда механика.
// Создаётся в MechanicWorkService и читается в MechanicOrdersViewModel.
public sealed class MechanicOrderDetails
{
    public string CarText { get; init; } = "";
    public string Fault { get; init; } = "";
    public string Engine { get; init; } = "";
    public string Brakes { get; init; } = "";
    public string Suspension { get; init; } = "";
    public string Electrical { get; init; } = "";
    public string Recommendations { get; init; } = "";
    public bool CanEditLines { get; init; }
    public IReadOnlyList<OrderLineRow> Services { get; init; } = [];
    public IReadOnlyList<OrderLineRow> Parts { get; init; } = [];
}

// Отзыв в списке.
// Создаётся в ReviewService и показывается в ReviewsViewModel.
public sealed class ReviewRow
{
    public int Id { get; init; }
    public int WorkOrderId { get; init; }
    public string CreatedAt { get; init; } = "";
    public string ClientName { get; init; } = "";
    public int Rating { get; init; }
    public string Comment { get; init; } = "";
}

// Одна строка показателя: название и значение.
// Создаётся в ChiefService и входит в AnalyticsSection.
public sealed class MetricRow
{
    public string Title { get; init; } = "";
    public string Value { get; init; } = "";
    public override string ToString() => string.IsNullOrEmpty(Value) ? Title : $"{Title}: {Value}";
}

// Блок аналитики с пояснением и строками.
// Создаётся в ChiefService и показывается в AnalyticsViewModel.
public sealed class AnalyticsSection
{
    public string Title { get; init; } = "";
    public string Explanation { get; init; } = "";
    public IReadOnlyList<MetricRow> Rows { get; init; } = [];
}
