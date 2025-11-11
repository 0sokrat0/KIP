using pr7.Data;
using pr7.Models;
using Microsoft.Data.Sqlite;

namespace pr7.Repositories;

public class PartRepository : IPartRepository
{
    private readonly DatabaseContext _dbContext;
    
    public PartRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Part GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Price FROM Parts WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Part
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2)
            };
        }
        
        throw new KeyNotFoundException($"Деталь с Id {id} не найдена");
    }
    
    public List<Part> GetAll()
    {
        var parts = new List<Part>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Price FROM Parts ORDER BY Name";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            parts.Add(new Part
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2)
            });
        }
        
        return parts;
    }
    
    public void Add(Part part)
    {
        if (string.IsNullOrWhiteSpace(part.Name))
            throw new ArgumentException("Название детали не может быть пустым");
        
        if (part.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Parts (Name, Price) VALUES (@Name, @Price)";
        command.Parameters.AddWithValue("@Name", part.Name);
        command.Parameters.AddWithValue("@Price", part.Price);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        part.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(Part part)
    {
        if (string.IsNullOrWhiteSpace(part.Name))
            throw new ArgumentException("Название детали не может быть пустым");
        
        if (part.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Parts SET Name = @Name, Price = @Price WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", part.Id);
        command.Parameters.AddWithValue("@Name", part.Name);
        command.Parameters.AddWithValue("@Price", part.Price);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Деталь с Id {part.Id} не найдена");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Parts WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Деталь с Id {id} не найдена");
    }
}

