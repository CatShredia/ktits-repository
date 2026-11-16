using Npgsql;
using ProductManagement;

namespace ProductManagement.Tests.Repositories;

// docker docker: run --hostname=16ec1a34644d --env=POSTGRES_PASSWORD=l41io83bpsvdewkfu0a7 --env=POSTGRES_DB=products_test --env=PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/usr/lib/postgresql/17/bin --env=GOSU_VERSION=1.19 --env=LANG=en_US.utf8 --env=PG_MAJOR=17 --env=PG_VERSION=17.12-1.pgdg13+1 --env=PGDATA=/var/lib/postgresql/data --volume=/var/lib/postgresql/data --network=bridge -p 54320:5432 --restart=no --runtime=runc -d postgres:17
public class ProductRepositoryTests : IDisposable
{
    // Строка по умолчанию для тестового Docker-контейнера products-test-db (только тестовая БД).
    private const string DefaultConnectionString =
        "Host=localhost;Port=54320;Database=products_test;Username=postgres;Password=l41io83bpsvdewkfu0a7";

    private readonly string _cs;
    private readonly ProductRepository _repo;

    public ProductRepositoryTests()
    {
        _cs = Environment.GetEnvironmentVariable("PRODUCTS_TEST_DB") ?? DefaultConnectionString;
        _repo = new ProductRepository(_cs);
        _repo.EnsureCreated();
        Clear();
    }

    private void Clear()
    {
        using var conn = new NpgsqlConnection(_cs);
        conn.Open();
        using var cmd = new NpgsqlCommand("TRUNCATE TABLE Products RESTART IDENTITY", conn);
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => Clear();

    private static Product NewProduct(string name = "Мышь") => new()
    {
        Name = name, Price = 1500.50m, Quantity = 10, Category = "Периферия"
    };

    [Fact]
    public void Add_NewProduct_AppearsInTable()
    {
        var p = NewProduct();
        var id = _repo.Add(p);

        Assert.True(id > 0);
        Assert.Equal(id, p.Id);
        Assert.Single(_repo.GetAll());
    }

    [Fact]
    public void GetById_ExistingProduct_ReturnsIt()
    {
        var id = _repo.Add(NewProduct());

        var found = _repo.GetById(id);

        Assert.NotNull(found);
        Assert.Equal(id, found!.Id);
        Assert.Equal("Мышь", found.Name);
        Assert.Equal(1500.50m, found.Price);
        Assert.Equal(10, found.Quantity);
        Assert.Equal("Периферия", found.Category);
    }

    [Fact]
    public void GetById_Missing_ReturnsNull()
        => Assert.Null(_repo.GetById(999999));

    [Fact]
    public void Update_ExistingProduct_ChangesData()
    {
        var p = NewProduct();
        _repo.Add(p);
        p.Name = "Клавиатура"; p.Price = 3000m; p.Quantity = 5; p.Category = "Ввод";

        Assert.True(_repo.Update(p));

        var updated = _repo.GetById(p.Id)!;
        Assert.Equal("Клавиатура", updated.Name);
        Assert.Equal(3000m, updated.Price);
        Assert.Equal(5, updated.Quantity);
        Assert.Equal("Ввод", updated.Category);
    }

    [Fact]
    public void Update_Missing_ReturnsFalse()
    {
        var p = NewProduct(); p.Id = 999999;
        Assert.False(_repo.Update(p));
    }

    [Fact]
    public void Delete_ExistingProduct_RemovesRecord()
    {
        var id = _repo.Add(NewProduct());

        Assert.True(_repo.Delete(id));
        Assert.Null(_repo.GetById(id));
    }

    [Fact]
    public void Delete_Missing_ReturnsFalse()
        => Assert.False(_repo.Delete(999999));

    [Fact]
    public void GetAll_ReturnsAllRecords()
    {
        _repo.Add(NewProduct("A"));
        _repo.Add(NewProduct("B"));
        _repo.Add(NewProduct("C"));

        var all = _repo.GetAll();

        Assert.Equal(3, all.Count);
        Assert.Equal(new[] { "A", "B", "C" }, all.Select(x => x.Name));
    }
}
