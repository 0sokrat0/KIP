using pr7.Data;
using pr7.Repositories;
using pr7.Services;

namespace pr7.Infrastructure;

public static class ServiceContainer
{
    private static DatabaseContext? _dbContext;
    private static ICarServiceFacade? _carServiceFacade;
    
    public static ICarServiceFacade GetCarServiceFacade()
    {
        if (_carServiceFacade != null)
            return _carServiceFacade;
        
        if (_dbContext == null)
        {
            string dbPath = "CarService.db";
            string connectionString = $"Data Source={dbPath};";
            _dbContext = new DatabaseContext(connectionString);
            _dbContext.InitializeDatabase();
        }
        
        var partRepository = new PartRepository(_dbContext);
        var warehouseRepository = new WarehouseRepository(_dbContext);
        var orderRepository = new OrderRepository(_dbContext);
        var clientRepository = new ClientRepository(_dbContext);
        var carRepository = new CarRepository(_dbContext);
        var carServiceRepository = new CarServiceRepository(_dbContext);
        
        var financeService = new FinanceService(carServiceRepository);
        var warehouseService = new WarehouseService(warehouseRepository);
        var orderService = new OrderService(orderRepository, financeService, warehouseService, partRepository);
        var clientService = new ClientService(partRepository, carRepository, clientRepository);
        var shopService = new ShopService(partRepository, warehouseService, financeService);
        
        _carServiceFacade = new CarServiceFacade(
            clientService,
            orderService,
            warehouseService,
            shopService,
            financeService,
            partRepository,
            carRepository);
        
        InitializeTestData(partRepository, warehouseService);
        
        return _carServiceFacade;
    }
    
    private static void InitializeTestData(IPartRepository partRepository, IWarehouseService warehouseService)
    {
        var existingParts = partRepository.GetAll();
        if (existingParts.Count > 0)
            return;
        
        var parts = new[]
        {
            new Models.Part { Name = "Тормозные колодки", Price = 2500.00m },
            new Models.Part { Name = "Масляный фильтр", Price = 800.00m },
            new Models.Part { Name = "Воздушный фильтр", Price = 600.00m },
            new Models.Part { Name = "Свечи зажигания", Price = 1200.00m },
            new Models.Part { Name = "Аккумулятор", Price = 5000.00m },
            new Models.Part { Name = "Генератор", Price = 8000.00m },
            new Models.Part { Name = "Стартер", Price = 6000.00m },
            new Models.Part { Name = "Радиатор", Price = 4500.00m },
            new Models.Part { Name = "Термостат", Price = 1500.00m },
            new Models.Part { Name = "Ремень ГРМ", Price = 3000.00m }
        };
        
        foreach (var part in parts)
        {
            partRepository.Add(part);
        }
        
        warehouseService.AddPart(1, 3);
        warehouseService.AddPart(2, 5);
        warehouseService.AddPart(3, 4);
        warehouseService.AddPart(4, 6);
        warehouseService.AddPart(5, 2);
    }
}

