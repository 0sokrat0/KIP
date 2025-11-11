using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _dbContext;
    
    public UserRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public User? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, Password FROM Users WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
        }
        
        return null;
    }
    
    public User? GetByUsername(string username)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, Password FROM Users WHERE Username = @Username";
        command.Parameters.AddWithValue("@Username", username);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            };
        }
        
        return null;
    }
    
    public List<User> GetAll()
    {
        var users = new List<User>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, Password FROM Users ORDER BY Username";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            });
        }
        
        return users;
    }
    
    public void Add(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentException("Имя пользователя не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ArgumentException("Пароль не может быть пустым");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Password", user.Password);
        
        try
        {
            command.ExecuteNonQuery();
            command.CommandText = "SELECT last_insert_rowid()";
            user.Id = Convert.ToInt32(command.ExecuteScalar());
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new ArgumentException("Пользователь с таким именем уже существует");
        }
    }
    
    public void Update(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentException("Имя пользователя не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(user.Password))
            throw new ArgumentException("Пароль не может быть пустым");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Users SET Username = @Username, Password = @Password WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Password", user.Password);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Пользователь с Id {user.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Users WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Пользователь с Id {id} не найден");
    }
}

