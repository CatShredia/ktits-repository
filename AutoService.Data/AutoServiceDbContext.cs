using AutoService.Data.Naming;
using AutoService.Data.Seed;

namespace AutoService.Data;

// Контекст Entity Framework: таблицы и сборка модели.
// Создаётся фабрикой в AppComposition и сервисах Workshop.
public sealed class AutoServiceDbContext : DbContext
{
    public AutoServiceDbContext(DbContextOptions<AutoServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<CarBrand> CarBrands => Set<CarBrand>();
    public DbSet<CarCategory> CarCategories => Set<CarCategory>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<MechanicService> MechanicServices => Set<MechanicService>();
    public DbSet<RepairBay> RepairBays => Set<RepairBay>();
    public DbSet<MechanicSchedule> MechanicSchedules => Set<MechanicSchedule>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderService> WorkOrderServices => Set<WorkOrderService>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<WorkOrderPart> WorkOrderParts => Set<WorkOrderPart>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<InspectionResult> InspectionResults => Set<InspectionResult>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AutoServiceDbContext).Assembly);
        SeedData.Apply(modelBuilder);

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(entity => entity.GetForeignKeys()))
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

        SnakeCaseNaming.Apply(modelBuilder);
    }
}
