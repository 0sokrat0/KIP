using pr7.Data;

namespace pr7;

class Program
{
    static void Main(string[] args)
    {
        string dbPath = "CarService.db";
        string connectionString = $"Data Source={dbPath};";
        
        var dbContext = new DatabaseContext(connectionString);
        
        Console.WriteLine("Инициализация базы данных...");
        dbContext.InitializeDatabase();
        Console.WriteLine("База данных успешно инициализирована!");
        
        Console.WriteLine("\nПроверка подключения к базе данных...");
        using var connection = dbContext.GetConnection();
        connection.Open();
        Console.WriteLine("Подключение к базе данных успешно!");
        connection.Close();
        
        Console.WriteLine("\nБаза данных готова к использованию!");
    }
}
