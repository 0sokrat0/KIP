using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DatabaseContext _dbContext;
    
    public CartRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<CartItem> GetByUserId(int userId)
    {
        var items = new List<CartItem>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId, ProductId, Quantity FROM CartItems WHERE UserId = @UserId";
        command.Parameters.AddWithValue("@UserId", userId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new CartItem
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3)
            });
        }
        
        return items;
    }
    
    public CartItem? GetByUserAndProduct(int userId, int productId)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId, ProductId, Quantity FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@ProductId", productId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new CartItem
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3)
            };
        }
        
        return null;
    }
    
    public void Add(CartItem item)
    {
        if (item.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO CartItems (UserId, ProductId, Quantity) VALUES (@UserId, @ProductId, @Quantity)";
        command.Parameters.AddWithValue("@UserId", item.UserId);
        command.Parameters.AddWithValue("@ProductId", item.ProductId);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        item.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(CartItem item)
    {
        if (item.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент корзины с Id {item.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM CartItems WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент корзины с Id {id} не найден");
    }
    
    public void ClearUserCart(int userId)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM CartItems WHERE UserId = @UserId";
        command.Parameters.AddWithValue("@UserId", userId);
        
        command.ExecuteNonQuery();
    }
}

