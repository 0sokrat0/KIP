using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly DatabaseContext _dbContext;
    
    public CartItemRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public CartItem? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CartId, ProductId, Quantity FROM CartItems WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new CartItem
            {
                Id = reader.GetInt32(0),
                CartId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3)
            };
        }
        
        return null;
    }
    
    public List<CartItem> GetByCartId(int cartId)
    {
        var cartItems = new List<CartItem>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CartId, ProductId, Quantity FROM CartItems WHERE CartId = @CartId";
        command.Parameters.AddWithValue("@CartId", cartId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cartItems.Add(new CartItem
            {
                Id = reader.GetInt32(0),
                CartId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3)
            });
        }
        
        return cartItems;
    }
    
    public CartItem? GetByCartIdAndProductId(int cartId, int productId)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CartId, ProductId, Quantity FROM CartItems WHERE CartId = @CartId AND ProductId = @ProductId";
        command.Parameters.AddWithValue("@CartId", cartId);
        command.Parameters.AddWithValue("@ProductId", productId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new CartItem
            {
                Id = reader.GetInt32(0),
                CartId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3)
            };
        }
        
        return null;
    }
    
    public void Add(CartItem cartItem)
    {
        if (cartItem.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO CartItems (CartId, ProductId, Quantity) VALUES (@CartId, @ProductId, @Quantity)";
        command.Parameters.AddWithValue("@CartId", cartItem.CartId);
        command.Parameters.AddWithValue("@ProductId", cartItem.ProductId);
        command.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
        
        try
        {
            command.ExecuteNonQuery();
            command.CommandText = "SELECT last_insert_rowid()";
            cartItem.Id = Convert.ToInt32(command.ExecuteScalar());
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            var existing = GetByCartIdAndProductId(cartItem.CartId, cartItem.ProductId);
            if (existing != null)
            {
                existing.Quantity += cartItem.Quantity;
                Update(existing);
                cartItem.Id = existing.Id;
                cartItem.Quantity = existing.Quantity;
            }
        }
    }
    
    public void Update(CartItem cartItem)
    {
        if (cartItem.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", cartItem.Id);
        command.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент корзины с Id {cartItem.Id} не найден");
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
    
    public void DeleteByCartId(int cartId)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM CartItems WHERE CartId = @CartId";
        command.Parameters.AddWithValue("@CartId", cartId);
        
        command.ExecuteNonQuery();
    }
}

