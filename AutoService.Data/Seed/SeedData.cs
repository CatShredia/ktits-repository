namespace AutoService.Data.Seed;

// Начальные роли, сотрудники, клиенты и один оплаченный заказ.
// Вызывается из AutoServiceDbContext.OnModelCreating.
internal static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Администратор" },
            new Role { Id = 2, Name = "Механик" },
            new Role { Id = 3, Name = "Руководитель" });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                RoleId = 1,
                FullName = "Иванова Мария Сергеевна",
                Login = "admin",
                PasswordHash = "100000.wZ5QTl7cXQpyQmhIilnjWQ==.DxQJHW9gnwkPa5t238+TMDp8s1VcYxrqyJYD+WXmPj0="
            },
            new User
            {
                Id = 2,
                RoleId = 2,
                FullName = "Петров Алексей Николаевич",
                Login = "mechanic",
                PasswordHash = "100000.iJdt0NmOptI11TdkBWEijQ==.62sQBvA3e2mayKIGA6Dvu9Z7dieB1zcJ66nM7VaOH24="
            },
            new User
            {
                Id = 3,
                RoleId = 2,
                FullName = "Сидоров Игорь Павлович",
                Login = "mechanic2",
                PasswordHash = "100000.Vvp5eKSeh5T+/+EILLQ21A==.ZfLw5GRwr6I1pxnouKpJBYYFXF9KhL/BkwXUXGhOhYQ="
            },
            new User
            {
                Id = 4,
                RoleId = 3,
                FullName = "Кузнецов Дмитрий Андреевич",
                Login = "chief",
                PasswordHash = "100000.D6Lp8Yio5nVp9seRgACV3Q==.TI4C15pFIO6Tx8659NCj6lc2qraufXW399Ly7eNRh0Q="
            });

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Слесарный цех" },
            new Department { Id = 2, Name = "Кузовной цех" });

        modelBuilder.Entity<Specialization>().HasData(
            new Specialization { Id = 1, Name = "Моторист" },
            new Specialization { Id = 2, Name = "Автоэлектрик" });

        modelBuilder.Entity<Mechanic>().HasData(
            new Mechanic { Id = 1, UserId = 2, DepartmentId = 1, SpecializationId = 1 },
            new Mechanic { Id = 2, UserId = 3, DepartmentId = 2, SpecializationId = 2 });

        modelBuilder.Entity<Client>().HasData(
            new Client { Id = 1, FullName = "Смирнов Олег Викторович", Phone = "+79001112233", Email = "smirnov@example.com" },
            new Client { Id = 2, FullName = "Орлова Анна Игоревна", Phone = "+79004445566", Email = "orlova@example.com" });

        modelBuilder.Entity<CarBrand>().HasData(
            new CarBrand { Id = 1, Name = "Toyota" },
            new CarBrand { Id = 2, Name = "Kia" });

        modelBuilder.Entity<CarCategory>().HasData(
            new CarCategory { Id = 1, Name = "Легковой" },
            new CarCategory { Id = 2, Name = "Кроссовер" });

        modelBuilder.Entity<Car>().HasData(
            new Car
            {
                Id = 1,
                ClientId = 1,
                CarBrandId = 1,
                CarCategoryId = 1,
                Model = "Camry",
                ManufactureYear = 2018,
                Vin = "JTM12345678900001",
                LicensePlate = "А123ВС116",
                Mileage = 82000,
                Color = "Белый",
                Notes = "Комплектация Comfort"
            },
            new Car
            {
                Id = 2,
                ClientId = 1,
                CarBrandId = 2,
                CarCategoryId = 2,
                Model = "Sportage",
                ManufactureYear = 2021,
                Vin = "XWE12345678900002",
                LicensePlate = "К456ОР116",
                Mileage = 34000,
                Color = "Серый"
            },
            new Car
            {
                Id = 3,
                ClientId = 2,
                CarBrandId = 1,
                CarCategoryId = 1,
                Model = "Corolla",
                ManufactureYear = 2016,
                Vin = "JTD12345678900003",
                LicensePlate = "М789ТТ116",
                Mileage = 141000,
                Color = "Синий"
            });

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = 1,
                Name = "Замена масла",
                Description = "Замена моторного масла и фильтра",
                BasePrice = 2500.00m,
                DurationMinutes = 40
            },
            new Service
            {
                Id = 2,
                Name = "Компьютерная диагностика",
                Description = "Считывание кодов ошибок электронных систем",
                BasePrice = 1800.00m,
                DurationMinutes = 30
            },
            new Service
            {
                Id = 3,
                Name = "Замена тормозных колодок",
                Description = "Замена колодок передней оси",
                BasePrice = 4200.00m,
                DurationMinutes = 90
            });

        modelBuilder.Entity<MechanicService>().HasData(
            new MechanicService { MechanicId = 1, ServiceId = 1 },
            new MechanicService { MechanicId = 1, ServiceId = 2 },
            new MechanicService { MechanicId = 2, ServiceId = 2 },
            new MechanicService { MechanicId = 2, ServiceId = 3 });

        modelBuilder.Entity<RepairBay>().HasData(
            new RepairBay { Id = 1, Name = "Подъёмник 1", Kind = RepairBayKind.Подъёмник },
            new RepairBay { Id = 2, Name = "Диагностический пост 1", Kind = RepairBayKind.ДиагностическийПост });

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "ООО «АвтоДеталь»", Phone = "+78432001122", Email = "sales@avtodetal.example" },
            new Supplier { Id = 2, Name = "ИП Каримов", Phone = "+79037654321", Email = "karimov@example.com" });

        modelBuilder.Entity<Part>().HasData(
            new Part
            {
                Id = 1,
                SupplierId = 1,
                Name = "Масляный фильтр",
                Sku = "FIL-001",
                PurchasePrice = 400.00m,
                SalePrice = 700.00m,
                Quantity = 12,
                MinQuantity = 4
            },
            new Part
            {
                Id = 2,
                SupplierId = 1,
                Name = "Моторное масло 5W-30",
                Sku = "OIL-5W30",
                PurchasePrice = 250.00m,
                SalePrice = 450.00m,
                Quantity = 10,
                MinQuantity = 4
            },
            new Part
            {
                Id = 3,
                SupplierId = 2,
                Name = "Тормозные колодки",
                Sku = "PAD-001",
                PurchasePrice = 1800.00m,
                SalePrice = 3200.00m,
                Quantity = 1,
                MinQuantity = 4
            });

        modelBuilder.Entity<MechanicSchedule>().HasData(
            new MechanicSchedule
            {
                Id = 1,
                MechanicId = 1,
                RepairBayId = 1,
                StartsAt = new DateTime(2026, 9, 15, 9, 0, 0),
                EndsAt = new DateTime(2026, 9, 15, 18, 0, 0)
            },
            new MechanicSchedule
            {
                Id = 2,
                MechanicId = 1,
                RepairBayId = 2,
                StartsAt = new DateTime(2026, 10, 20, 9, 0, 0),
                EndsAt = new DateTime(2026, 10, 20, 18, 0, 0)
            },
            new MechanicSchedule
            {
                Id = 3,
                MechanicId = 2,
                RepairBayId = 1,
                StartsAt = new DateTime(2026, 9, 16, 9, 0, 0),
                EndsAt = new DateTime(2026, 9, 16, 18, 0, 0)
            });

        modelBuilder.Entity<Appointment>().HasData(
            new Appointment
            {
                Id = 1,
                ClientId = 1,
                CarId = 1,
                ServiceId = 1,
                MechanicId = 1,
                RepairBayId = 1,
                ScheduledAt = new DateTime(2026, 9, 15, 10, 0, 0),
                Status = AppointmentStatus.Выполнена
            },
            new Appointment
            {
                Id = 2,
                ClientId = 2,
                CarId = 3,
                ServiceId = 2,
                MechanicId = 1,
                RepairBayId = 2,
                ScheduledAt = new DateTime(2026, 10, 20, 11, 0, 0),
                Status = AppointmentStatus.Запланирована
            });

        modelBuilder.Entity<WorkOrder>().HasData(
            new WorkOrder
            {
                Id = 1,
                AppointmentId = 1,
                MechanicId = 1,
                CreatedAt = new DateTime(2026, 9, 15, 10, 20, 0),
                Status = WorkOrderStatus.Завершён,
                FaultDescription = "Пониженный уровень моторного масла, загрязнён масляный фильтр."
            });

        modelBuilder.Entity<WorkOrderService>().HasData(
            new WorkOrderService { WorkOrderId = 1, ServiceId = 1, Price = 2500.00m });

        modelBuilder.Entity<WorkOrderPart>().HasData(
            new WorkOrderPart { WorkOrderId = 1, PartId = 1, Quantity = 1, UnitPrice = 700.00m },
            new WorkOrderPart { WorkOrderId = 1, PartId = 2, Quantity = 4, UnitPrice = 450.00m });

        modelBuilder.Entity<InspectionResult>().HasData(
            new InspectionResult
            {
                Id = 1,
                WorkOrderId = 1,
                EngineState = "Уровень масла ниже нормы, фильтр загрязнён",
                BrakeState = "Износа нет",
                SuspensionState = "Без замечаний",
                ElectricalState = "Ошибок электронных систем нет",
                Recommendations = "Повторить замену масла через 10 000 км"
            });

        modelBuilder.Entity<Invoice>().HasData(
            new Invoice
            {
                Id = 1,
                WorkOrderId = 1,
                CreatedAt = new DateTime(2026, 9, 15, 16, 0, 0),
                AmountBeforeDiscount = 5000.00m,
                Discount = 500.00m,
                TaxRate = 20.00m,
                Tax = 900.00m,
                Total = 5400.00m,
                Status = InvoiceStatus.Оплачен
            });

        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                Id = 1,
                InvoiceId = 1,
                PaidAt = new DateTime(2026, 9, 15, 16, 30, 0),
                Amount = 5400.00m,
                Method = PaymentMethod.Карта,
                Status = PaymentStatus.Проведён,
                TransactionNumber = "TRX-20260915-0001"
            });

        modelBuilder.Entity<Review>().HasData(
            new Review
            {
                Id = 1,
                ClientId = 1,
                WorkOrderId = 1,
                Rating = 5,
                Comment = "Заменили масло быстро, объяснили состояние фильтра.",
                CreatedAt = new DateTime(2026, 9, 16, 12, 0, 0)
            });
    }
}
