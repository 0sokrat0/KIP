using Microsoft.Data.Sqlite;

namespace pr8.Data;

public class DatabaseContext
{
    private readonly string _connectionString;
    
    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public void InitializeDatabase()
    {
        CreateTables();
    }
    
    private void CreateTables()
    {
        using var connection = GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Price DECIMAL(10,2) NOT NULL CHECK(Price >= 0),
                Stock INTEGER NOT NULL CHECK(Stock >= 0)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS PickupPoints (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Address TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS CartItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK(Quantity > 0),
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
                UNIQUE(UserId, ProductId)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                PickupPointId INTEGER NOT NULL,
                OrderDate TEXT NOT NULL,
                TotalAmount DECIMAL(10,2) NOT NULL CHECK(TotalAmount >= 0),
                FOREIGN KEY (UserId) REFERENCES Users(Id),
                FOREIGN KEY (PickupPointId) REFERENCES PickupPoints(Id)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS OrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK(Quantity > 0),
                Price DECIMAL(10,2) NOT NULL CHECK(Price >= 0),
                FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
        ";
        command.ExecuteNonQuery();
    }
    
    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}

