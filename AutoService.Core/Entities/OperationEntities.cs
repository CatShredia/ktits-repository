using AutoService.Core.Enums;

namespace AutoService.Core.Entities;

// Смена механика на ремонтном месте.
// Хранится в AutoServiceDbContext, ведётся в AdminService.
public sealed class MechanicSchedule
{
    public int Id { get; set; }
    public int MechanicId { get; set; }
    public int RepairBayId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public Mechanic Mechanic { get; set; } = null!;
    public RepairBay RepairBay { get; set; } = null!;
}

// Запись клиента на услугу.
// Хранится в AutoServiceDbContext, ведётся в AdminService и MechanicWorkService.
public sealed class Appointment
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int CarId { get; set; }
    public int ServiceId { get; set; }
    public int MechanicId { get; set; }
    public int RepairBayId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public AppointmentStatus Status { get; set; }
    public Client Client { get; set; } = null!;
    public Car Car { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public Mechanic Mechanic { get; set; } = null!;
    public RepairBay RepairBay { get; set; } = null!;
    public WorkOrder? WorkOrder { get; set; }
}

// Заказ-наряд по записи.
// Хранится в AutoServiceDbContext, ведётся механиком и смотрится администратором.
public sealed class WorkOrder
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int MechanicId { get; set; }
    public DateTime CreatedAt { get; set; }
    public WorkOrderStatus Status { get; set; }
    public string? FaultDescription { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public Mechanic Mechanic { get; set; } = null!;
    public ICollection<WorkOrderService> Services { get; set; } = new List<WorkOrderService>();
    public ICollection<WorkOrderPart> Parts { get; set; } = new List<WorkOrderPart>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public InspectionResult? Inspection { get; set; }
    public Review? Review { get; set; }
}

// Услуга в составе заказ-наряда.
// Хранится в AutoServiceDbContext, добавляется в MechanicWorkService.
public sealed class WorkOrderService
{
    public int WorkOrderId { get; set; }
    public int ServiceId { get; set; }
    public decimal Price { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public Service Service { get; set; } = null!;
}

// Запчасть, списанная в заказ-наряд.
// Хранится в AutoServiceDbContext, добавляется в MechanicWorkService.
public sealed class WorkOrderPart
{
    public int WorkOrderId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public Part Part { get; set; } = null!;
}

// Результат осмотра по заказ-наряду.
// Хранится в AutoServiceDbContext, сохраняется в MechanicWorkService.
public sealed class InspectionResult
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public required string EngineState { get; set; }
    public required string BrakeState { get; set; }
    public required string SuspensionState { get; set; }
    public required string ElectricalState { get; set; }
    public required string Recommendations { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
}
