using Microsoft.EntityFrameworkCore;
using ProductionSystem.Data;

namespace ProductionSystem.Api.Tests.Infrastructure;

internal static class TestDataSeeder
{
    public static async Task ClearAsync(AppDbContext db)
    {
        db.ProductMeasurements.RemoveRange(await db.ProductMeasurements.ToListAsync());
        db.ProductDrawings.RemoveRange(await db.ProductDrawings.ToListAsync());
        db.OrderQualityChecks.RemoveRange(await db.OrderQualityChecks.ToListAsync());
        db.EquipmentFailures.RemoveRange(await db.EquipmentFailures.ToListAsync());
        db.WorkshopLayoutItems.RemoveRange(await db.WorkshopLayoutItems.ToListAsync());
        db.OrderStatusHistory.RemoveRange(await db.OrderStatusHistory.ToListAsync());
        db.OrderDimensions.RemoveRange(await db.OrderDimensions.ToListAsync());
        db.CustomerOrders.RemoveRange(await db.CustomerOrders.ToListAsync());
        db.ProductMaterialSpecs.RemoveRange(await db.ProductMaterialSpecs.ToListAsync());
        db.ProductComponentSpecs.RemoveRange(await db.ProductComponentSpecs.ToListAsync());
        db.ProductOperationSpecs.RemoveRange(await db.ProductOperationSpecs.ToListAsync());
        db.ProductAssemblySpecs.RemoveRange(await db.ProductAssemblySpecs.ToListAsync());
        db.Products.RemoveRange(await db.Products.ToListAsync());
        db.WorkerOperations.RemoveRange(await db.WorkerOperations.ToListAsync());
        db.Workers.RemoveRange(await db.Workers.ToListAsync());
        db.ProductionOperations.RemoveRange(await db.ProductionOperations.ToListAsync());
        db.Materials.RemoveRange(await db.Materials.ToListAsync());
        db.Components.RemoveRange(await db.Components.ToListAsync());
        db.Equipment.RemoveRange(await db.Equipment.ToListAsync());
        db.EquipmentTypes.RemoveRange(await db.EquipmentTypes.ToListAsync());
        db.Workshops.RemoveRange(await db.Workshops.ToListAsync());
        db.Warehouses.RemoveRange(await db.Warehouses.ToListAsync());
        db.Suppliers.RemoveRange(await db.Suppliers.ToListAsync());
        db.Users.RemoveRange(await db.Users.ToListAsync());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    public const string CustomerLogin = "test_customer";
    public const string CustomerPassword = "Test1";
    public const string ManagerLogin = "test_manager";
    public const string ManagerPassword = "Test1";
    public const string DirectorLogin = "test_director";
    public const string DirectorPassword = "Test1";
    public const string ForemanLogin = "test_foreman";
    public const string ForemanPassword = "Test1";

    public static async Task SeedAsync(AppDbContext db)
    {
        db.Users.AddRange(
            new AppUser { Login = CustomerLogin, Password = CustomerPassword, Role = UserRoles.Customer, FullName = "Тест Заказчик" },
            new AppUser { Login = ManagerLogin, Password = ManagerPassword, Role = UserRoles.Manager, FullName = "Тест Менеджер" },
            new AppUser { Login = DirectorLogin, Password = DirectorPassword, Role = UserRoles.Director, FullName = "Тест Директор" },
            new AppUser { Login = ForemanLogin, Password = ForemanPassword, Role = UserRoles.Foreman, FullName = "Тест Мастер" });

        var warehouse = new Warehouse { Name = "Склад №1" };
        db.Warehouses.Add(warehouse);
        await db.SaveChangesAsync();

        var material = new Material
        {
            Article = "MAT-001",
            Name = "Сталь листовая",
            Unit = "кг",
            Quantity = 100,
            MaterialType = "Металл",
            PurchasePrice = 50,
            WarehouseId = warehouse.Id,
        };
        db.Materials.Add(material);

        db.Components.Add(new StockComponent
        {
            Article = "CMP-001",
            Name = "Подшипник",
            Unit = "шт",
            Quantity = 20,
            ComponentType = "Механика",
            PurchasePrice = 120,
            Weight = 1,
            WarehouseId = warehouse.Id,
        });

        var product = new Product { Name = "Изделие тестовое", Dimensions = "100x50" };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        db.ProductMaterialSpecs.Add(new ProductMaterialSpec
        {
            ProductName = product.Name,
            MaterialId = material.Id,
            Quantity = 5,
        });

        db.CustomerOrders.Add(new CustomerOrder
        {
            Number = "ЗК-TEST-0001",
            OrderName = "Тестовый заказ",
            OrderDate = DateOnly.FromDateTime(DateTime.Today),
            ProductName = product.Name,
            ProductDescription = "Описание",
            CustomerLogin = CustomerLogin,
            ManagerLogin = ManagerLogin,
            Status = OrderStatuses.Specification,
        });

        db.Workshops.Add(new Workshop { Name = "Сборочный цех" });

        await db.SaveChangesAsync();
    }
}
