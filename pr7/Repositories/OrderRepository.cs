using pr7.Data;
using pr7.Models;
using Microsoft.Data.Sqlite;

namespace pr7.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DatabaseContext _dbContext;
    
    public OrderRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Order GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ClientId, CarId, BrokenPartId, RepairCost, Status, CreatedAt FROM Orders WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Order
            {
                Id = reader.GetInt32(0),
                ClientId = reader.GetInt32(1),
                CarId = reader.GetInt32(2),
                BrokenPartId = reader.GetInt32(3),
                RepairCost = reader.GetDecimal(4),
                Status = (OrderStatus)reader.GetInt32(5),
                CreatedAt = DateTime.Parse(reader.GetString(6))
            };
        }
        
        throw new KeyNotFoundException($"Заказ с Id {id} не найден");
    }
    
    public List<Order> GetAll()
    {
        var orders = new List<Order>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ClientId, CarId, BrokenPartId, RepairCost, Status, CreatedAt FROM Orders ORDER BY CreatedAt DESC";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            orders.Add(new Order
            {
                Id = reader.GetInt32(0),
                ClientId = reader.GetInt32(1),
                CarId = reader.GetInt32(2),
                BrokenPartId = reader.GetInt32(3),
                RepairCost = reader.GetDecimal(4),
                Status = (OrderStatus)reader.GetInt32(5),
                CreatedAt = DateTime.Parse(reader.GetString(6))
            });
        }
        
        return orders;
    }
    
    public void Add(Order order)
    {
        if (order.RepairCost < 0)
            throw new ArgumentException("Стоимость ремонта не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Orders (ClientId, CarId, BrokenPartId, RepairCost, Status, CreatedAt) VALUES (@ClientId, @CarId, @BrokenPartId, @RepairCost, @Status, @CreatedAt)";
        command.Parameters.AddWithValue("@ClientId", order.ClientId);
        command.Parameters.AddWithValue("@CarId", order.CarId);
        command.Parameters.AddWithValue("@BrokenPartId", order.BrokenPartId);
        command.Parameters.AddWithValue("@RepairCost", order.RepairCost);
        command.Parameters.AddWithValue("@Status", (int)order.Status);
        command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        order.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(Order order)
    {
        if (order.RepairCost < 0)
            throw new ArgumentException("Стоимость ремонта не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Orders SET ClientId = @ClientId, CarId = @CarId, BrokenPartId = @BrokenPartId, RepairCost = @RepairCost, Status = @Status, CreatedAt = @CreatedAt WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", order.Id);
        command.Parameters.AddWithValue("@ClientId", order.ClientId);
        command.Parameters.AddWithValue("@CarId", order.CarId);
        command.Parameters.AddWithValue("@BrokenPartId", order.BrokenPartId);
        command.Parameters.AddWithValue("@RepairCost", order.RepairCost);
        command.Parameters.AddWithValue("@Status", (int)order.Status);
        command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Заказ с Id {order.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Orders WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Заказ с Id {id} не найден");
    }
}

