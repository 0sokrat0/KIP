using pr8.Data;
using pr8.Repositories;
using pr8.Services;

namespace pr8.Infrastructure;

public static class ServiceContainer
{
    private static DatabaseContext? _dbContext;
    private static IMarketplaceFacade? _marketplaceFacade;
    
    public static IMarketplaceFacade GetMarketplaceFacade()
    {
        if (_marketplaceFacade != null)
            return _marketplaceFacade;
        
        if (_dbContext == null)
        {
            string dbPath = "Marketplace.db";
            string connectionString = $"Data Source={dbPath};";
            _dbContext = new DatabaseContext(connectionString);
            _dbContext.InitializeDatabase();
        }
        
        var userRepository = new UserRepository(_dbContext);
        var productRepository = new ProductRepository(_dbContext);
        var pickupPointRepository = new PickupPointRepository(_dbContext);
        var cartRepository = new CartRepository(_dbContext);
        var orderRepository = new OrderRepository(_dbContext);
        var orderItemRepository = new OrderItemRepository(_dbContext);
        
        var authService = new AuthService(userRepository);
        var productService = new ProductService(productRepository);
        var cartService = new CartService(cartRepository, productRepository);
        var orderService = new OrderService(orderRepository, orderItemRepository, productRepository, cartRepository);
        var pickupPointService = new PickupPointService(pickupPointRepository);
        
        _marketplaceFacade = new MarketplaceFacade(
            authService,
            productService,
            cartService,
            orderService,
            pickupPointService,
            productRepository);
        
        InitializeTestData(productRepository, pickupPointRepository);
        
        return _marketplaceFacade;
    }
    
    private static void InitializeTestData(IProductRepository productRepository, IPickupPointRepository pickupPointRepository)
    {
        var existingProducts = productRepository.GetAll();
        if (existingProducts.Count > 0)
            return;
        
        var products = new[]
        {
            new Models.Product { Name = "Смартфон Samsung Galaxy", Description = "Современный смартфон с отличной камерой", Price = 25000.00m, Stock = 15 },
            new Models.Product { Name = "Ноутбук ASUS", Description = "Мощный ноутбук для работы и игр", Price = 55000.00m, Stock = 8 },
            new Models.Product { Name = "Наушники Sony WH-1000XM4", Description = "Беспроводные наушники с шумоподавлением", Price = 18000.00m, Stock = 20 },
            new Models.Product { Name = "Планшет iPad Air", Description = "Легкий и быстрый планшет", Price = 45000.00m, Stock = 12 },
            new Models.Product { Name = "Умные часы Apple Watch", Description = "Фитнес-трекер и смарт-часы", Price = 30000.00m, Stock = 10 },
            new Models.Product { Name = "Клавиатура Logitech MX", Description = "Беспроводная механическая клавиатура", Price = 8000.00m, Stock = 25 },
            new Models.Product { Name = "Мышь Razer DeathAdder", Description = "Игровая мышь с оптическим сенсором", Price = 3500.00m, Stock = 30 },
            new Models.Product { Name = "Монитор LG 27 дюймов", Description = "4K монитор с IPS матрицей", Price = 22000.00m, Stock = 5 }
        };
        
        using (var connection = _dbContext!.GetConnection())
        {
            connection.Open();
            foreach (var product in products)
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Products (Name, Description, Price, Stock) VALUES (@Name, @Description, @Price, @Stock)";
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.ExecuteNonQuery();
            }
        }
        
        var pickupPoints = new[]
        {
            new Models.PickupPoint { Name = "ПВЗ на Тверской", Address = "г. Москва, ул. Тверская, д. 10" },
            new Models.PickupPoint { Name = "ПВЗ на Арбате", Address = "г. Москва, ул. Арбат, д. 25" },
            new Models.PickupPoint { Name = "ПВЗ на Ленинском", Address = "г. Москва, Ленинский проспект, д. 50" },
            new Models.PickupPoint { Name = "ПВЗ на Садовом кольце", Address = "г. Москва, Садовое кольцо, д. 15" }
        };
        
        using (var connection = _dbContext!.GetConnection())
        {
            connection.Open();
            foreach (var point in pickupPoints)
            {
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO PickupPoints (Name, Address) VALUES (@Name, @Address)";
                command.Parameters.AddWithValue("@Name", point.Name);
                command.Parameters.AddWithValue("@Address", point.Address);
                command.ExecuteNonQuery();
            }
        }
    }
}

