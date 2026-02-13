using Dapper;
using netcrud.Models;
using MySqlConnector;
using System.Data;

namespace netcrud.Data;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();



    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string missing.");
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<Product>("SELECT * FROM Products ORDER BY Id");
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Product>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<Product> CreateAsync(Product product)
    {
        using var conn = CreateConnection();
        const string sql = @"
            INSERT INTO Products (Name, Price, Stock)
            VALUES (@Name, @Price, @Stock);
            SELECT LAST_INSERT_ID();";

        var newId = await conn.ExecuteScalarAsync<int>(sql, product);
        product.Id = newId;
        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var conn = CreateConnection();
        const string sql = @"
            UPDATE Products 
            SET Name = @Name, Price = @Price, Stock = @Stock
            WHERE Id = @Id";

        var rows = await conn.ExecuteAsync(sql, product);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "DELETE FROM Products WHERE Id = @Id",
            new { Id = id });

        return rows > 0;
    }
}