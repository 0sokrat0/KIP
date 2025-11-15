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
    
    public List<OrderItem> GetByOrderId(int orderId)
    {
        var items = new List<OrderItem>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, OrderId, ProductId, Quantity, Price FROM OrderItems WHERE OrderId = @OrderId";
        command.Parameters.AddWithValue("@OrderId", orderId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new OrderItem
            {
                Id = reader.GetInt32(0),
                OrderId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                Price = reader.GetDecimal(4)
            });
        }
        
        return items;
    }
    
    public void Add(OrderItem item)
    {
        if (item.Quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        if (item.Price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) VALUES (@OrderId, @ProductId, @Quantity, @Price)";
        command.Parameters.AddWithValue("@OrderId", item.OrderId);
        command.Parameters.AddWithValue("@ProductId", item.ProductId);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        command.Parameters.AddWithValue("@Price", item.Price);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        item.Id = Convert.ToInt32(command.ExecuteScalar());
    }
}

