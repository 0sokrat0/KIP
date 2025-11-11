using pr7.Data;
using pr7.Models;
using Microsoft.Data.Sqlite;

namespace pr7.Repositories;

public class CarServiceRepository : ICarServiceRepository
{
    private readonly DatabaseContext _dbContext;
    
    public CarServiceRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public CarService Get()
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Balance FROM CarService WHERE Id = 1";
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new CarService
            {
                Id = reader.GetInt32(0),
                Balance = reader.GetDecimal(1)
            };
        }
        
        var carService = new CarService { Id = 1, Balance = 10000.00m };
        Update(carService);
        return carService;
    }
    
    public void Update(CarService carService)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT OR REPLACE INTO CarService (Id, Balance) VALUES (@Id, @Balance)";
        command.Parameters.AddWithValue("@Id", carService.Id);
        command.Parameters.AddWithValue("@Balance", carService.Balance);
        
        command.ExecuteNonQuery();
    }
}

