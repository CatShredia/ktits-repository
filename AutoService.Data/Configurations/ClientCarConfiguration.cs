namespace AutoService.Data.Configurations;

// Правила таблицы клиентов.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients", table => table.HasComment("Клиенты"));
        builder.Property(x => x.FullName).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(FieldLimits.Phone).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(FieldLimits.Email);
        builder.HasIndex(x => x.FullName);
        builder.HasIndex(x => x.Phone);
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

// Правила таблицы марок.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class CarBrandConfiguration : IEntityTypeConfiguration<CarBrand>
{
    public void Configure(EntityTypeBuilder<CarBrand> builder)
    {
        builder.ToTable("car_brands", table => table.HasComment("Марки автомобилей"));
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

// Правила таблицы категорий автомобилей.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class CarCategoryConfiguration : IEntityTypeConfiguration<CarCategory>
{
    public void Configure(EntityTypeBuilder<CarCategory> builder)
    {
        builder.ToTable("car_categories", table => table.HasComment("Категории автомобилей"));
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

// Правила таблицы автомобилей.
// Подключается в AutoServiceDbContext.OnModelCreating.
internal sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("cars", table =>
        {
            table.HasComment("Автомобили");
            table.HasCheckConstraint("ck_cars_year", "manufacture_year >= 1970 and manufacture_year <= 2100");
            table.HasCheckConstraint("ck_cars_mileage", "mileage >= 0");
            table.HasCheckConstraint("ck_cars_vin_length", "char_length(vin) = 17");
        });
        builder.Property(x => x.Model).HasMaxLength(FieldLimits.Model).IsRequired();
        builder.Property(x => x.Vin).HasMaxLength(FieldLimits.Vin).IsRequired();
        builder.Property(x => x.LicensePlate).HasMaxLength(FieldLimits.LicensePlate).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(FieldLimits.Color);
        builder.Property(x => x.Notes).HasMaxLength(FieldLimits.Notes);
        builder.HasIndex(x => x.Vin).IsUnique();
        builder.HasIndex(x => x.LicensePlate).IsUnique();
        builder.HasIndex(x => x.Mileage);
        builder.HasAlternateKey(x => new { x.Id, x.ClientId }).HasName("ak_cars_id_client_id");
        builder.HasOne(x => x.Client).WithMany(x => x.Cars).HasForeignKey(x => x.ClientId);
        builder.HasOne(x => x.Brand).WithMany(x => x.Cars).HasForeignKey(x => x.CarBrandId);
        builder.HasOne(x => x.Category).WithMany(x => x.Cars).HasForeignKey(x => x.CarCategoryId);
    }
}
