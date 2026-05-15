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
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
    public DbSet<ProductMaterialSpec> ProductMaterialSpecs => Set<ProductMaterialSpec>();
    public DbSet<ProductComponentSpec> ProductComponentSpecs => Set<ProductComponentSpec>();
    public DbSet<ProductOperationSpec> ProductOperationSpecs => Set<ProductOperationSpec>();
    public DbSet<ProductAssemblySpec> ProductAssemblySpecs => Set<ProductAssemblySpec>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<OrderStatusHistory> OrderStatusHistory => Set<OrderStatusHistory>();
    public DbSet<OrderDimension> OrderDimensions => Set<OrderDimension>();
    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<WorkshopLayoutItem> WorkshopLayoutItems => Set<WorkshopLayoutItem>();
    public DbSet<EquipmentFailure> EquipmentFailures => Set<EquipmentFailure>();
    public DbSet<OrderQualityCheck> OrderQualityChecks => Set<OrderQualityCheck>();

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
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Article, x.WarehouseId }).IsUnique();
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
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Article, x.WarehouseId });
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

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("products");
            e.HasKey(x => x.Name);
            e.Property(x => x.Name).HasMaxLength(512);
            e.Property(x => x.Dimensions).HasMaxLength(1024).IsRequired();
        });

        modelBuilder.Entity<CustomerOrder>(e =>
        {
            e.ToTable("customer_orders");
            e.HasKey(x => x.Number);
            e.Property(x => x.Number).HasMaxLength(64);
            e.Property(x => x.OrderName).HasMaxLength(512).IsRequired();
            e.Property(x => x.ProductName).HasMaxLength(512).IsRequired();
            e.Property(x => x.CustomerLogin).HasMaxLength(64).IsRequired();
            e.Property(x => x.ManagerLogin).HasMaxLength(64);
            e.Property(x => x.Status).HasMaxLength(64).IsRequired();
            e.Property(x => x.RejectionReason).HasMaxLength(2000);
            e.Property(x => x.ProductDescription).HasMaxLength(4000);
            e.HasOne(x => x.Product).WithMany(p => p.CustomerOrders).HasForeignKey(x => x.ProductName)
                .HasPrincipalKey(p => p.Name).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerLogin).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Manager).WithMany().HasForeignKey(x => x.ManagerLogin).IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrderStatusHistory>(e =>
        {
            e.ToTable("order_status_history");
            e.HasKey(x => x.Id);
            e.Property(x => x.OrderNumber).HasMaxLength(64).IsRequired();
            e.Property(x => x.Status).HasMaxLength(64).IsRequired();
            e.Property(x => x.ChangedByLogin).HasMaxLength(64);
            e.Property(x => x.Comment).HasMaxLength(2000);
            e.HasOne(x => x.Order).WithMany(o => o.StatusHistory).HasForeignKey(x => x.OrderNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderDimension>(e =>
        {
            e.ToTable("order_dimensions");
            e.HasKey(x => x.Id);
            e.Property(x => x.OrderNumber).HasMaxLength(64).IsRequired();
            e.Property(x => x.Description).HasMaxLength(512);
            e.Property(x => x.Unit).HasMaxLength(64);
            e.HasOne(x => x.Order).WithMany(o => o.Dimensions).HasForeignKey(x => x.OrderNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Workshop>(e =>
        {
            e.ToTable("workshops");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<WorkshopLayoutItem>(e =>
        {
            e.ToTable("workshop_layout_items");
            e.HasKey(x => x.Id);
            e.Property(x => x.IconType).HasMaxLength(64).IsRequired();
            e.HasOne(x => x.Workshop).WithMany(w => w.LayoutItems).HasForeignKey(x => x.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EquipmentFailure>(e =>
        {
            e.ToTable("equipment_failures");
            e.HasKey(x => x.Id);
            e.Property(x => x.EquipmentMarking).HasMaxLength(256).IsRequired();
            e.Property(x => x.Reason).HasMaxLength(2000).IsRequired();
            e.Property(x => x.RegisteredByLogin).HasMaxLength(64).IsRequired();
            e.HasOne(x => x.Equipment).WithMany().HasForeignKey(x => x.EquipmentMarking)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.RegisteredBy).WithMany().HasForeignKey(x => x.RegisteredByLogin)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderQualityCheck>(e =>
        {
            e.ToTable("order_quality_checks");
            e.HasKey(x => x.Id);
            e.Property(x => x.OrderNumber).HasMaxLength(64).IsRequired();
            e.Property(x => x.ParameterName).HasMaxLength(512).IsRequired();
            e.Property(x => x.Grade).HasMaxLength(8).IsRequired();
            e.Property(x => x.Comment).HasMaxLength(2000);
            e.Property(x => x.CheckedByLogin).HasMaxLength(64).IsRequired();
            e.HasOne(x => x.Order).WithMany(o => o.QualityChecks).HasForeignKey(x => x.OrderNumber)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductMaterialSpec>(e =>
        {
            e.ToTable("product_material_specs");
            e.HasKey(x => new { x.ProductName, x.MaterialId });
            e.Property(x => x.ProductName).HasMaxLength(512);
            e.HasOne(x => x.Product).WithMany(p => p.MaterialSpecs).HasForeignKey(x => x.ProductName)
                .HasPrincipalKey(p => p.Name).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductComponentSpec>(e =>
        {
            e.ToTable("product_component_specs");
            e.HasKey(x => new { x.ProductName, x.ComponentId });
            e.Property(x => x.ProductName).HasMaxLength(512);
            e.HasOne(x => x.Product).WithMany(p => p.ComponentSpecs).HasForeignKey(x => x.ProductName)
                .HasPrincipalKey(p => p.Name).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Component).WithMany().HasForeignKey(x => x.ComponentId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductOperationSpec>(e =>
        {
            e.ToTable("product_operation_specs");
            e.HasKey(x => new { x.ProductName, x.OperationId, x.SequenceNumber });
            e.Property(x => x.ProductName).HasMaxLength(512);
            e.Property(x => x.EquipmentTypeName).HasMaxLength(256);
            e.HasOne(x => x.Product).WithMany(p => p.OperationSpecs).HasForeignKey(x => x.ProductName)
                .HasPrincipalKey(p => p.Name).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Operation).WithMany(o => o.ProductOperationSpecs).HasForeignKey(x => x.OperationId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.EquipmentType).WithMany(t => t.OperationSpecs).HasForeignKey(x => x.EquipmentTypeName)
                .HasPrincipalKey(t => t.Name).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductAssemblySpec>(e =>
        {
            e.ToTable("product_assembly_specs");
            e.HasKey(x => new { x.ParentProductName, x.ChildProductName });
            e.Property(x => x.ParentProductName).HasMaxLength(512);
            e.Property(x => x.ChildProductName).HasMaxLength(512);
            e.HasOne(x => x.ParentProduct).WithMany(p => p.AssemblyChildren).HasForeignKey(x => x.ParentProductName)
                .HasPrincipalKey(p => p.Name).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.ChildProduct).WithMany(p => p.AssemblyParents).HasForeignKey(x => x.ChildProductName)
                .HasPrincipalKey(p => p.Name).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EquipmentType>(e =>
        {
            e.ToTable("equipment_types");
            e.HasKey(x => x.Name);
            e.Property(x => x.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<Equipment>(e =>
        {
            e.ToTable("equipment");
            e.HasKey(x => x.Marking);
            e.Property(x => x.Marking).HasMaxLength(256);
            e.Property(x => x.EquipmentTypeName).HasMaxLength(256).IsRequired();
            e.Property(x => x.Characteristics).HasMaxLength(4000);
            e.HasOne(x => x.EquipmentType).WithMany(t => t.Equipment).HasForeignKey(x => x.EquipmentTypeName)
                .HasPrincipalKey(t => t.Name).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
