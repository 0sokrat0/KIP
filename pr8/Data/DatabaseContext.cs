using Microsoft.Data.Sqlite;

namespace pr8.Data;

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
        SeedInitialData();
    }
    
    public void CreateTables()
    {
        using var connection = GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Price DECIMAL(10,2) NOT NULL CHECK(Price >= 0),
                Description TEXT
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
            CREATE TABLE IF NOT EXISTS Carts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                UNIQUE(UserId)
            );
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS CartItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CartId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL CHECK(Quantity > 0),
                FOREIGN KEY (CartId) REFERENCES Carts(Id) ON DELETE CASCADE,
                FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
                UNIQUE(CartId, ProductId)
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
    
    public void SeedInitialData()
    {
        using var connection = GetConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        
        command.CommandText = @"
            INSERT OR IGNORE INTO Products (Name, Price, Description) VALUES
            ('Ноутбук ASUS', 45000.00, '15.6 дюймов, Intel Core i5, 8GB RAM'),
            ('Смартфон Samsung', 25000.00, '6.1 дюймов, 128GB, камера 48MP'),
            ('Наушники Sony', 5000.00, 'Беспроводные, шумоподавление'),
            ('Планшет iPad', 35000.00, '10.2 дюймов, 64GB'),
            ('Клавиатура Logitech', 3000.00, 'Механическая, RGB подсветка'),
            ('Мышь Razer', 2500.00, 'Игровая, оптический сенсор'),
            ('Монитор LG', 15000.00, '27 дюймов, 4K, IPS'),
            ('Веб-камера Logitech', 4000.00, 'Full HD, автофокус');
        ";
        command.ExecuteNonQuery();
        
        command.CommandText = @"
            INSERT OR IGNORE INTO PickupPoints (Name, Address) VALUES
            ('ПВЗ на Ленина', 'ул. Ленина, д. 10'),
            ('ПВЗ на Пушкина', 'ул. Пушкина, д. 25'),
            ('ПВЗ на Гагарина', 'пр. Гагарина, д. 5'),
            ('ПВЗ Центральный', 'пл. Центральная, д. 1');
        ";
        command.ExecuteNonQuery();
    }
    
    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
