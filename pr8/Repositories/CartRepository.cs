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
    
    public Cart? GetByUserId(int userId)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId FROM Carts WHERE UserId = @UserId";
        command.Parameters.AddWithValue("@UserId", userId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Cart
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1)
            };
        }
        
        return null;
    }
    
    public Cart? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId FROM Carts WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Cart
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1)
            };
        }
        
        return null;
    }
    
    public void Add(Cart cart)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Carts (UserId) VALUES (@UserId)";
        command.Parameters.AddWithValue("@UserId", cart.UserId);
        
        try
        {
            command.ExecuteNonQuery();
            command.CommandText = "SELECT last_insert_rowid()";
            cart.Id = Convert.ToInt32(command.ExecuteScalar());
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new ArgumentException("Корзина для этого пользователя уже существует");
        }
    }
    
    public void Update(Cart cart)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Carts SET UserId = @UserId WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", cart.Id);
        command.Parameters.AddWithValue("@UserId", cart.UserId);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Корзина с Id {cart.Id} не найдена");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Carts WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Корзина с Id {id} не найдена");
    }
}

