using pr7.Data;
using pr7.Models;
using Microsoft.Data.Sqlite;

namespace pr7.Repositories;

public class CarRepository : ICarRepository
{
    private readonly DatabaseContext _dbContext;
    
    public CarRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Car GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Model, BrokenPartId FROM Cars WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Car
            {
                Id = reader.GetInt32(0),
                Model = reader.GetString(1),
                BrokenPartId = reader.GetInt32(2)
            };
        }
        
        throw new KeyNotFoundException($"Машина с Id {id} не найдена");
    }
    
    public List<Car> GetAll()
    {
        var cars = new List<Car>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Model, BrokenPartId FROM Cars ORDER BY Model";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cars.Add(new Car
            {
                Id = reader.GetInt32(0),
                Model = reader.GetString(1),
                BrokenPartId = reader.GetInt32(2)
            });
        }
        
        return cars;
    }
    
    public void Add(Car car)
    {
        if (string.IsNullOrWhiteSpace(car.Model))
            throw new ArgumentException("Модель машины не может быть пустой");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Cars (Model, BrokenPartId) VALUES (@Model, @BrokenPartId)";
        command.Parameters.AddWithValue("@Model", car.Model);
        command.Parameters.AddWithValue("@BrokenPartId", car.BrokenPartId);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        car.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(Car car)
    {
        if (string.IsNullOrWhiteSpace(car.Model))
            throw new ArgumentException("Модель машины не может быть пустой");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Cars SET Model = @Model, BrokenPartId = @BrokenPartId WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", car.Id);
        command.Parameters.AddWithValue("@Model", car.Model);
        command.Parameters.AddWithValue("@BrokenPartId", car.BrokenPartId);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Машина с Id {car.Id} не найдена");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Cars WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Машина с Id {id} не найдена");
    }
}

