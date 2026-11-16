using Npgsql;

namespace ProductManagement;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public const string CreateTableSql = @"
CREATE TABLE IF NOT EXISTS Products
(
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(150) NOT NULL,
    Price NUMERIC(10, 2) NOT NULL,
    Quantity INTEGER NOT NULL,
    Category VARCHAR(100) NOT NULL
);";

    public void EnsureCreated()
    {
        using var conn = Open();
        using var cmd = new NpgsqlCommand(CreateTableSql, conn);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Добавляет товар и записывает присвоенный Id в product.Id.</summary>
    public int Add(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        using var conn = Open();
        using var cmd = new NpgsqlCommand(
            "INSERT INTO Products (Name, Price, Quantity, Category) VALUES (@n, @p, @q, @c) RETURNING Id", conn);
        Fill(cmd, product);
        product.Id = (int)cmd.ExecuteScalar()!;
        return product.Id;
    }

    /// <summary>Возвращает товар или null, если его нет.</summary>
    public Product? GetById(int id)
    {
        using var conn = Open();
        using var cmd = new NpgsqlCommand("SELECT Id, Name, Price, Quantity, Category FROM Products WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Map(r) : null;
    }

    public List<Product> GetAll()
    {
        var list = new List<Product>();
        using var conn = Open();
        using var cmd = new NpgsqlCommand("SELECT Id, Name, Price, Quantity, Category FROM Products ORDER BY Id", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(Map(r));
        return list;
    }

    /// <summary>true, если запись найдена и обновлена.</summary>
    public bool Update(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        using var conn = Open();
        using var cmd = new NpgsqlCommand(
            "UPDATE Products SET Name=@n, Price=@p, Quantity=@q, Category=@c WHERE Id=@id", conn);
        Fill(cmd, product);
        cmd.Parameters.AddWithValue("id", product.Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    /// <summary>true, если запись найдена и удалена.</summary>
    public bool Delete(int id)
    {
        using var conn = Open();
        using var cmd = new NpgsqlCommand("DELETE FROM Products WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }

    private NpgsqlConnection Open()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        return conn;
    }

    private static void Fill(NpgsqlCommand cmd, Product p)
    {
        cmd.Parameters.AddWithValue("n", p.Name);
        cmd.Parameters.AddWithValue("p", p.Price);
        cmd.Parameters.AddWithValue("q", p.Quantity);
        cmd.Parameters.AddWithValue("c", p.Category);
    }

    private static Product Map(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        Name = r.GetString(1),
        Price = r.GetDecimal(2),
        Quantity = r.GetInt32(3),
        Category = r.GetString(4)
    };
}
