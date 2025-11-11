using pr7.Data;
using pr7.Models;
using Microsoft.Data.Sqlite;

namespace pr7.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly DatabaseContext _dbContext;
    
    public ClientRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Client GetById(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, CarId FROM Clients WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Client
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                CarId = reader.GetInt32(2)
            };
        }
        
        throw new KeyNotFoundException($"Клиент с Id {id} не найден");
    }
    
    public List<Client> GetAll()
    {
        var clients = new List<Client>();
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, CarId FROM Clients ORDER BY Name";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            clients.Add(new Client
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                CarId = reader.GetInt32(2)
            });
        }
        
        return clients;
    }
    
    public void Add(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name))
            throw new ArgumentException("Имя клиента не может быть пустым");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Clients (Name, CarId) VALUES (@Name, @CarId)";
        command.Parameters.AddWithValue("@Name", client.Name);
        command.Parameters.AddWithValue("@CarId", client.CarId);
        
        command.ExecuteNonQuery();
        
        command.CommandText = "SELECT last_insert_rowid()";
        client.Id = Convert.ToInt32(command.ExecuteScalar());
    }
    
    public void Update(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Name))
            throw new ArgumentException("Имя клиента не может быть пустым");
        
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Clients SET Name = @Name, CarId = @CarId WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", client.Id);
        command.Parameters.AddWithValue("@Name", client.Name);
        command.Parameters.AddWithValue("@CarId", client.CarId);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Клиент с Id {client.Id} не найден");
    }
    
    public void Delete(int id)
    {
        using var connection = _dbContext.GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Clients WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        
        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Клиент с Id {id} не найден");
    }
}

