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
        var points = new List<PickupPoint>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Address FROM PickupPoints ORDER BY Name";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            points.Add(new PickupPoint
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2)
            });
        }
        
        return points;
    }
}

