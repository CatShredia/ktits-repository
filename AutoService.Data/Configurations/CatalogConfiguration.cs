using AutoService.Data.Conversions;

namespace AutoService.Data.Configurations;

internal sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services", table =>
        {
            table.HasComment("Услуги");
            table.HasCheckConstraint("ck_services_price", "base_price >= 0");
            table.HasCheckConstraint("ck_services_duration", "duration_minutes > 0");
        });
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(FieldLimits.Description).IsRequired();
        builder.Property(x => x.BasePrice).HasPrecision(12, 2);
        builder.HasIndex(x => x.BasePrice);
        builder.HasIndex(x => x.DurationMinutes);
    }
}

internal sealed class MechanicServiceConfiguration : IEntityTypeConfiguration<MechanicService>
{
    public void Configure(EntityTypeBuilder<MechanicService> builder)
    {
        builder.ToTable("mechanic_services", table => table.HasComment("Связь механиков и услуг"));
        builder.HasKey(x => new { x.MechanicId, x.ServiceId });
        builder.HasOne(x => x.Mechanic).WithMany(x => x.Services).HasForeignKey(x => x.MechanicId);
        builder.HasOne(x => x.Service).WithMany(x => x.Mechanics).HasForeignKey(x => x.ServiceId);
    }
}

internal sealed class RepairBayConfiguration : IEntityTypeConfiguration<RepairBay>
{
    public void Configure(EntityTypeBuilder<RepairBay> builder)
    {
        builder.ToTable("repair_bays", table =>
        {
            table.HasComment("Ремонтные места");
            table.HasCheckConstraint(
                "ck_repair_bays_kind",
                "kind in ('Подъёмник', 'Диагностический пост', 'Шиномонтажная зона', 'Кузовной участок')");
        });
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.Property(x => x.Kind).HasConversion(StatusConverters.RepairBayKind).HasMaxLength(FieldLimits.Status);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

internal sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers", table => table.HasComment("Поставщики"));
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(FieldLimits.Phone);
        builder.Property(x => x.Email).HasMaxLength(FieldLimits.Email);
    }
}

internal sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.ToTable("parts", table =>
        {
            table.HasComment("Запчасти");
            table.HasCheckConstraint("ck_parts_purchase_price", "purchase_price >= 0");
            table.HasCheckConstraint("ck_parts_sale_price", "sale_price >= 0");
            table.HasCheckConstraint("ck_parts_quantity", "quantity >= 0");
            table.HasCheckConstraint("ck_parts_min_quantity", "min_quantity >= 0");
        });
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.Property(x => x.Sku).HasMaxLength(FieldLimits.Sku).IsRequired();
        builder.Property(x => x.PurchasePrice).HasPrecision(12, 2);
        builder.Property(x => x.SalePrice).HasPrecision(12, 2);
        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasIndex(x => x.Quantity);
        builder.HasOne(x => x.Supplier).WithMany(x => x.Parts).HasForeignKey(x => x.SupplierId);
    }
}
