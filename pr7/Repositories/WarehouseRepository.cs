using pr7.Data;
using pr7.Models;
using Microsoft.Data.Sqlite;

namespace pr7.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly DatabaseContext _dbContext;
    
    public WarehouseRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public WarehouseItem GetByPartId(int partId)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PartId, Quantity, DeliveryDate FROM WarehouseItems WHERE PartId = @PartId";
        command.Parameters.AddWithValue("@PartId", partId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new WarehouseItem
            {
                Id = reader.GetInt32(0),
                PartId = reader.GetInt32(1),
                Quantity = reader.GetInt32(2),
                DeliveryDate = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3))
            };
        }
        
        return null!;
    }
    
    public List<WarehouseItem> GetAll()
    {
        var items = new List<WarehouseItem>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PartId, Quantity, DeliveryDate FROM WarehouseItems WHERE DeliveryDate IS NULL OR DeliveryDate <= datetime('now') ORDER BY PartId";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new WarehouseItem
            {
                Id = reader.GetInt32(0),
                PartId = reader.GetInt32(1),
                Quantity = reader.GetInt32(2),
                DeliveryDate = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3))
            });
        }
        
        return items;
    }
    
    public List<WarehouseItem> GetPendingDeliveries()
    {
        var items = new List<WarehouseItem>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, PartId, Quantity, DeliveryDate FROM WarehouseItems WHERE DeliveryDate IS NOT NULL AND DeliveryDate > datetime('now') ORDER BY DeliveryDate";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new WarehouseItem
            {
                Id = reader.GetInt32(0),
                PartId = reader.GetInt32(1),
                Quantity = reader.GetInt32(2),
                DeliveryDate = DateTime.Parse(reader.GetString(3))
            });
        }
        
        return items;
    }
    
    public void Add(WarehouseItem item)
    {
        if (item.Quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO WarehouseItems (PartId, Quantity, DeliveryDate) VALUES (@PartId, @Quantity, @DeliveryDate)";
        command.Parameters.AddWithValue("@PartId", item.PartId);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        
        var deliveryDateParam = command.CreateParameter();
        deliveryDateParam.ParameterName = "@DeliveryDate";
        deliveryDateParam.Value = item.DeliveryDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value;
        command.Parameters.Add(deliveryDateParam);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        item.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(WarehouseItem item)
    {
        if (item.Quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE WarehouseItems SET PartId = @PartId, Quantity = @Quantity, DeliveryDate = @DeliveryDate WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@PartId", item.PartId);
        command.Parameters.AddWithValue("@Quantity", item.Quantity);
        
        var deliveryDateParam = command.CreateParameter();
        deliveryDateParam.ParameterName = "@DeliveryDate";
        deliveryDateParam.Value = item.DeliveryDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value;
        command.Parameters.Add(deliveryDateParam);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент склада с Id {item.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM WarehouseItems WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Элемент склада с Id {id} не найден");
    }
}

