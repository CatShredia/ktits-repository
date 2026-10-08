namespace AutoService.Core.Entities;

// Роль сотрудника.
// Хранится в AutoServiceDbContext, настраивается в RoleConfiguration.
public sealed class Role
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}

// Учётная запись сотрудника.
// Хранится в AutoServiceDbContext, читается в AuthService.
public sealed class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public required string FullName { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public Role Role { get; set; } = null!;
    public Mechanic? Mechanic { get; set; }
}

// Отдел автосервиса.
// Хранится в AutoServiceDbContext, выбирается для механика.
public sealed class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}

// Специализация механика.
// Хранится в AutoServiceDbContext, выбирается для механика.
public sealed class Specialization
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}

// Механик: пользователь, отдел и специализация.
// Хранится в AutoServiceDbContext, используется в записи и заказ-наряде.
public sealed class Mechanic
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public int SpecializationId { get; set; }
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public Specialization Specialization { get; set; } = null!;
    public ICollection<MechanicService> Services { get; set; } = new List<MechanicService>();
    public ICollection<MechanicSchedule> Schedules { get; set; } = new List<MechanicSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
