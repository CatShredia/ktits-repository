using AutoService.Core.Enums;

namespace AutoService.Core.Entities;

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

public sealed class MechanicService
{
    public int MechanicId { get; set; }
    public int ServiceId { get; set; }
    public Mechanic Mechanic { get; set; } = null!;
    public Service Service { get; set; } = null!;
}

public sealed class RepairBay
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public RepairBayKind Kind { get; set; }
    public ICollection<MechanicSchedule> Schedules { get; set; } = new List<MechanicSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

public sealed class Supplier
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}

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
