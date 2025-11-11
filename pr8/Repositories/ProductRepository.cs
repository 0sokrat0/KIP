using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DatabaseContext _dbContext;
    
    public ProductRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Product? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Price, Description FROM Products WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
            };
        }
        
        return null;
    }
    
    public List<Product> GetAll()
    {
        var products = new List<Product>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Price, Description FROM Products ORDER BY Name";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
            });
        }
        
        return products;
    }
    
    public void Add(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Название товара не может быть пустым");
        
        if (product.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Products (Name, Price, Description) VALUES (@Name, @Price, @Description)";
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        product.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Название товара не может быть пустым");
        
        if (product.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Products SET Name = @Name, Price = @Price, Description = @Description WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Товар с Id {product.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Products WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Товар с Id {id} не найден");
    }
}

