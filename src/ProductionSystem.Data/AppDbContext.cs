using Microsoft.EntityFrameworkCore;

namespace ProductionSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<StockComponent> Components => Set<StockComponent>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<ProductionOperation> ProductionOperations => Set<ProductionOperation>();
    public DbSet<WorkerOperation> WorkerOperations => Set<WorkerOperation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Login);
            e.Property(x => x.Login).HasMaxLength(64);
            e.Property(x => x.Password).HasMaxLength(256);
            e.Property(x => x.Role).HasMaxLength(32);
            e.Property(x => x.FullName).HasMaxLength(512);
        });

        modelBuilder.Entity<Supplier>(e =>
        {
            e.ToTable("suppliers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(512).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Warehouse>(e =>
        {
            e.ToTable("warehouses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
        });

        modelBuilder.Entity<Material>(e =>
        {
            e.ToTable("materials");
            e.HasKey(x => x.Article);
            e.Property(x => x.Article).HasMaxLength(64);
            e.Property(x => x.Name).HasMaxLength(1024);
            e.Property(x => x.Unit).HasMaxLength(64);
            e.Property(x => x.MaterialType).HasMaxLength(256);
            e.Property(x => x.Gost).HasMaxLength(256);
            e.Property(x => x.Characteristics).HasMaxLength(4000);
            e.HasOne(x => x.Supplier).WithMany(x => x.Materials).HasForeignKey(x => x.SupplierId);
            e.HasOne(x => x.Warehouse).WithMany(x => x.Materials).HasForeignKey(x => x.WarehouseId);
        });

        modelBuilder.Entity<StockComponent>(e =>
        {
            e.ToTable("components");
            e.HasKey(x => x.Article);
            e.Property(x => x.Article).HasMaxLength(64);
            e.Property(x => x.Name).HasMaxLength(1024);
            e.Property(x => x.Unit).HasMaxLength(64);
            e.Property(x => x.ComponentType).HasMaxLength(256);
            e.HasOne(x => x.Supplier).WithMany(x => x.Components).HasForeignKey(x => x.SupplierId);
            e.HasOne(x => x.Warehouse).WithMany(x => x.Components).HasForeignKey(x => x.WarehouseId);
        });

        modelBuilder.Entity<Worker>(e =>
        {
            e.ToTable("workers");
            e.HasKey(x => x.Id);
            e.Property(x => x.LastName).HasMaxLength(256);
            e.Property(x => x.FirstMiddleName).HasMaxLength(512);
            e.Property(x => x.HomeAddress).HasMaxLength(1024);
            e.Property(x => x.Education).HasMaxLength(512);
            e.Property(x => x.Qualification).HasMaxLength(512);
        });

        modelBuilder.Entity<ProductionOperation>(e =>
        {
            e.ToTable("production_operations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<WorkerOperation>(e =>
        {
            e.ToTable("worker_operations");
            e.HasKey(x => new { x.WorkerId, x.OperationId });
            e.HasOne(x => x.Worker).WithMany(x => x.WorkerOperations).HasForeignKey(x => x.WorkerId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Operation).WithMany(x => x.WorkerOperations).HasForeignKey(x => x.OperationId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
