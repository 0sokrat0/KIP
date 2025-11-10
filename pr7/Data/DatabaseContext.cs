using Microsoft.Data.Sqlite;

namespace pr7.Data;

public class DatabaseContext
{
    private string _connectionString;
    
    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public void InitializeDatabase()
    {
        CreateTables();
    }
    
    public void CreateTables()
    {
        using var connection = GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Parts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price DECIMAL(10,2) NOT NULL CHECK(Price >= 0)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Cars (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Model TEXT NOT NULL,
                BrokenPartId INTEGER NOT NULL,
                FOREIGN KEY (BrokenPartId) REFERENCES Parts(Id)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Clients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                CarId INTEGER NOT NULL,
                FOREIGN KEY (CarId) REFERENCES Cars(Id)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClientId INTEGER NOT NULL,
                CarId INTEGER NOT NULL,
                BrokenPartId INTEGER NOT NULL,
                RepairCost DECIMAL(10,2) NOT NULL CHECK(RepairCost >= 0),
                Status INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (ClientId) REFERENCES Clients(Id),
                FOREIGN KEY (CarId) REFERENCES Cars(Id),
                FOREIGN KEY (BrokenPartId) REFERENCES Parts(Id)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS WarehouseItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PartId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK(Quantity >= 0),
                DeliveryDate TEXT,
                FOREIGN KEY (PartId) REFERENCES Parts(Id)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS CarService (
                Id INTEGER PRIMARY KEY,
                Balance DECIMAL(10,2) NOT NULL DEFAULT 0
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            INSERT OR IGNORE INTO CarService (Id, Balance) VALUES (1, 10000.00);
        ";
        command.ExecuteNonQuery();
    }
    
    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}

