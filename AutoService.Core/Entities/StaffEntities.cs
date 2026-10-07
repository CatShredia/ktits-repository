namespace AutoService.Core.Entities;

public sealed class Role
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}

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

public sealed class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}

public sealed class Specialization
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}

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
