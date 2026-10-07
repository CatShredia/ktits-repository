using AutoService.Data.Conversions;

namespace AutoService.Data.Configurations;

internal sealed class MechanicScheduleConfiguration : IEntityTypeConfiguration<MechanicSchedule>
{
    public void Configure(EntityTypeBuilder<MechanicSchedule> builder)
    {
        builder.ToTable("mechanic_schedules", table =>
        {
            table.HasComment("Расписание механиков");
            table.HasCheckConstraint("ck_mechanic_schedules_period", "ends_at > starts_at");
        });
        builder.Property(x => x.StartsAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.EndsAt).HasColumnType("timestamp without time zone");
        builder.HasOne(x => x.Mechanic).WithMany(x => x.Schedules).HasForeignKey(x => x.MechanicId);
        builder.HasOne(x => x.RepairBay).WithMany(x => x.Schedules).HasForeignKey(x => x.RepairBayId);
    }
}

internal sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments", table =>
        {
            table.HasComment("Записи на обслуживание");
            table.HasCheckConstraint(
                "ck_appointments_status",
                "status in ('Запланирована', 'Подтверждена', 'Отменена', 'Выполнена')");
        });
        builder.Property(x => x.ScheduledAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(FieldLimits.Status);
        builder.HasIndex(x => x.ScheduledAt);
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Client).WithMany(x => x.Appointments).HasForeignKey(x => x.ClientId);
        builder.HasOne(x => x.Car)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => new { x.CarId, x.ClientId })
            .HasPrincipalKey(x => new { x.Id, x.ClientId });
        builder.HasOne(x => x.Service).WithMany(x => x.Appointments).HasForeignKey(x => x.ServiceId);
        builder.HasOne(x => x.Mechanic).WithMany(x => x.Appointments).HasForeignKey(x => x.MechanicId);
        builder.HasOne(x => x.RepairBay).WithMany(x => x.Appointments).HasForeignKey(x => x.RepairBayId);
    }
}

internal sealed class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("work_orders", table =>
        {
            table.HasComment("Заказ-наряды");
            table.HasCheckConstraint(
                "ck_work_orders_status",
                "status in ('Создан', 'Диагностика', 'В ремонте', 'Завершён', 'Отменён')");
        });
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp without time zone");
        builder.Property(x => x.Status).HasConversion(StatusConverters.WorkOrderStatus).HasMaxLength(FieldLimits.Status);
        builder.Property(x => x.FaultDescription).HasMaxLength(FieldLimits.Fault);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Appointment).WithOne(x => x.WorkOrder).HasForeignKey<WorkOrder>(x => x.AppointmentId);
        builder.HasOne(x => x.Mechanic).WithMany(x => x.WorkOrders).HasForeignKey(x => x.MechanicId);
    }
}

internal sealed class WorkOrderServiceConfiguration : IEntityTypeConfiguration<WorkOrderService>
{
    public void Configure(EntityTypeBuilder<WorkOrderService> builder)
    {
        builder.ToTable("work_order_services", table =>
        {
            table.HasComment("Услуги в заказ-нарядах");
            table.HasCheckConstraint("ck_work_order_services_price", "price >= 0");
        });
        builder.HasKey(x => new { x.WorkOrderId, x.ServiceId });
        builder.Property(x => x.Price).HasPrecision(12, 2);
        builder.HasOne(x => x.WorkOrder).WithMany(x => x.Services).HasForeignKey(x => x.WorkOrderId);
        builder.HasOne(x => x.Service).WithMany(x => x.WorkOrderLines).HasForeignKey(x => x.ServiceId);
    }
}

internal sealed class WorkOrderPartConfiguration : IEntityTypeConfiguration<WorkOrderPart>
{
    public void Configure(EntityTypeBuilder<WorkOrderPart> builder)
    {
        builder.ToTable("work_order_parts", table =>
        {
            table.HasComment("Запчасти в заказ-нарядах");
            table.HasCheckConstraint("ck_work_order_parts_quantity", "quantity > 0");
            table.HasCheckConstraint("ck_work_order_parts_price", "unit_price >= 0");
        });
        builder.HasKey(x => new { x.WorkOrderId, x.PartId });
        builder.Property(x => x.UnitPrice).HasPrecision(12, 2);
        builder.HasOne(x => x.WorkOrder).WithMany(x => x.Parts).HasForeignKey(x => x.WorkOrderId);
        builder.HasOne(x => x.Part).WithMany(x => x.WorkOrderLines).HasForeignKey(x => x.PartId);
    }
}

internal sealed class InspectionResultConfiguration : IEntityTypeConfiguration<InspectionResult>
{
    public void Configure(EntityTypeBuilder<InspectionResult> builder)
    {
        builder.ToTable("inspection_results", table => table.HasComment("Результаты технического осмотра"));
        builder.Property(x => x.EngineState).HasMaxLength(FieldLimits.Inspection).IsRequired();
        builder.Property(x => x.BrakeState).HasMaxLength(FieldLimits.Inspection).IsRequired();
        builder.Property(x => x.SuspensionState).HasMaxLength(FieldLimits.Inspection).IsRequired();
        builder.Property(x => x.ElectricalState).HasMaxLength(FieldLimits.Inspection).IsRequired();
        builder.Property(x => x.Recommendations).HasMaxLength(FieldLimits.Inspection).IsRequired();
        builder.HasIndex(x => x.WorkOrderId).IsUnique();
        builder.HasOne(x => x.WorkOrder).WithOne(x => x.Inspection).HasForeignKey<InspectionResult>(x => x.WorkOrderId);
    }
}
