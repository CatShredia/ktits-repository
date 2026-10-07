namespace AutoService.Core.Entities;

public sealed class Client
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public sealed class CarBrand
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}

public sealed class CarCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Car> Cars { get; set; } = new List<Car>();
}

public sealed class Car
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int CarBrandId { get; set; }
    public int CarCategoryId { get; set; }
    public required string Model { get; set; }
    public int ManufactureYear { get; set; }
    public required string Vin { get; set; }
    public required string LicensePlate { get; set; }
    public int Mileage { get; set; }
    public string? Color { get; set; }
    public string? Notes { get; set; }
    public Client Client { get; set; } = null!;
    public CarBrand Brand { get; set; } = null!;
    public CarCategory Category { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
