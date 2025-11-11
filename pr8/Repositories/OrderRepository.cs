using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DatabaseContext _dbContext;
    
    public OrderRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Order? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId, PickupPointId, OrderDate, TotalAmount FROM Orders WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Order
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                PickupPointId = reader.GetInt32(2),
                OrderDate = DateTime.Parse(reader.GetString(3)),
                TotalAmount = reader.GetDecimal(4)
            };
        }
        
        return null;
    }
    
    public List<Order> GetByUserId(int userId)
    {
        var orders = new List<Order>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, UserId, PickupPointId, OrderDate, TotalAmount FROM Orders WHERE UserId = @UserId";
        command.Parameters.AddWithValue("@UserId", userId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            orders.Add(new Order
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                PickupPointId = reader.GetInt32(2),
                OrderDate = DateTime.Parse(reader.GetString(3)),
                TotalAmount = reader.GetDecimal(4)
            });
        }
        
        return orders;
    }
    
    public void Add(Order order)
    {
        if (order.TotalAmount < 0)
            throw new ArgumentException("Сумма заказа не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Orders (UserId, PickupPointId, OrderDate, TotalAmount) VALUES (@UserId, @PickupPointId, @OrderDate, @TotalAmount)";
        command.Parameters.AddWithValue("@UserId", order.UserId);
        command.Parameters.AddWithValue("@PickupPointId", order.PickupPointId);
        command.Parameters.AddWithValue("@OrderDate", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        order.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(Order order)
    {
        if (order.TotalAmount < 0)
            throw new ArgumentException("Сумма заказа не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Orders SET UserId = @UserId, PickupPointId = @PickupPointId, OrderDate = @OrderDate, TotalAmount = @TotalAmount WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", order.Id);
        command.Parameters.AddWithValue("@UserId", order.UserId);
        command.Parameters.AddWithValue("@PickupPointId", order.PickupPointId);
        command.Parameters.AddWithValue("@OrderDate", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
        
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

