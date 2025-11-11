using pr8.Data;
using pr8.Models;
using Microsoft.Data.Sqlite;

namespace pr8.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly DatabaseContext _dbContext;
    
    public OrderItemRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public OrderItem? GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, OrderId, ProductId, Quantity, Price FROM OrderItems WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new OrderItem
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                Price = reader.GetDecimal(4)
            };
        }
        
        return null;
    }
    
    public List<OrderItem> GetByOrderId(int orderId)
    {
        var orderItems = new List<OrderItem>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, OrderId, ProductId, Quantity, Price FROM OrderItems WHERE OrderId = @OrderId";
        command.Parameters.AddWithValue("@OrderId", orderId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            orderItems.Add(new OrderItem
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                Price = reader.GetDecimal(4)
            });
        }
        
        return orderItems;
    }
    
    public void Add(OrderItem orderItem)
    {
        if (orderItem.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        if (orderItem.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) VALUES (@OrderId, @ProductId, @Quantity, @Price)";
        command.Parameters.AddWithValue("@OrderId", orderItem.OrderId);
        command.Parameters.AddWithValue("@ProductId", orderItem.ProductId);
        command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
        command.Parameters.AddWithValue("@Price", orderItem.Price);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        orderItem.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(OrderItem orderItem)
    {
        if (orderItem.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        if (orderItem.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE OrderItems SET Quantity = @Quantity, Price = @Price WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", orderItem.Id);
        command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
        command.Parameters.AddWithValue("@Price", orderItem.Price);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент заказа с Id {orderItem.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM OrderItems WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент заказа с Id {id} не найден");
    }
}

