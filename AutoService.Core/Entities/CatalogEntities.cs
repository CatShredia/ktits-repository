using AutoService.Core.Enums;

namespace AutoService.Core.Entities;

// Услуга с ценой и длительностью.
// Хранится в AutoServiceDbContext, выбирается в записи и заказе.
public sealed class Service
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal BasePrice { get; set; }
    public int DurationMinutes { get; set; }
    public ICollection<MechanicService> Mechanics { get; set; } = new List<MechanicService>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<WorkOrderService> WorkOrderLines { get; set; } = new List<WorkOrderService>();
}

// Связь механика с услугой, которую он выполняет.
// Хранится в AutoServiceDbContext, проверяется при записи и в справочнике.
public sealed class MechanicService
{
    public int MechanicId { get; set; }
    public int ServiceId { get; set; }
    public Mechanic Mechanic { get; set; } = null!;
    public Service Service { get; set; } = null!;
}

// Ремонтное место.
// Хранится в AutoServiceDbContext, выбирается в записи и расписании.
public sealed class RepairBay
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public RepairBayKind Kind { get; set; }
    public ICollection<MechanicSchedule> Schedules { get; set; } = new List<MechanicSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

// Поставщик запчастей.
// Хранится в AutoServiceDbContext, выбирается у запчасти.
public sealed class Supplier
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}

// Запчасть и её остаток на складе.
// Хранится в AutoServiceDbContext, списывается в MechanicWorkService.
public sealed class Part
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public int MinQuantity { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public ICollection<WorkOrderPart> WorkOrderLines { get; set; } = new List<WorkOrderPart>();
}
