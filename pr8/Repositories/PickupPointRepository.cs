using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class PickupPointRepository : IPickupPointRepository
{
    private readonly DatabaseContext _dbContext;
    
    public PickupPointRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public PickupPoint? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Address FROM PickupPoints WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new PickupPoint
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2)
            };
        }
        
        return null;
    }
    
    public List<PickupPoint> GetAll()
    {
        var pickupPoints = new List<PickupPoint>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Address FROM PickupPoints ORDER BY Name";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            pickupPoints.Add(new PickupPoint
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2)
            });
        }
        
        return pickupPoints;
    }
    
    public void Add(PickupPoint pickupPoint)
    {
        if (string.IsNullOrWhiteSpace(pickupPoint.Name))
            throw new ArgumentException("Название ПВЗ не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(pickupPoint.Address))
            throw new ArgumentException("Адрес ПВЗ не может быть пустым");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO PickupPoints (Name, Address) VALUES (@Name, @Address)";
        command.Parameters.AddWithValue("@Name", pickupPoint.Name);
        command.Parameters.AddWithValue("@Address", pickupPoint.Address);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        pickupPoint.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(PickupPoint pickupPoint)
    {
        if (string.IsNullOrWhiteSpace(pickupPoint.Name))
            throw new ArgumentException("Название ПВЗ не может быть пустым");
        
        if (string.IsNullOrWhiteSpace(pickupPoint.Address))
            throw new ArgumentException("Адрес ПВЗ не может быть пустым");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE PickupPoints SET Name = @Name, Address = @Address WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", pickupPoint.Id);
        command.Parameters.AddWithValue("@Name", pickupPoint.Name);
        command.Parameters.AddWithValue("@Address", pickupPoint.Address);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"ПВЗ с Id {pickupPoint.Id} не найдено");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM PickupPoints WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"ПВЗ с Id {id} не найдено");
    }
}

