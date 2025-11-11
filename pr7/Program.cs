using System.Linq;
using pr7.Data;
using pr7.Models;
using pr7.Repositories;
using pr7.Services;

namespace pr7;

class Program
{
    private static DatabaseContext? _dbContext;
    private static FinanceService? _financeService;
    private static WarehouseService? _warehouseService;
    private static OrderService? _orderService;
    private static ClientService? _clientService;
    private static ShopService? _shopService;
    private static IPartRepository? _partRepository;
    private static ICarRepository? _carRepository;
    private static int _carsProcessed = 0;
    
    static void Main(string[] args)
    {
        try
        {
            InitializeGame();
            InitializeTestData();
            
            Console.WriteLine("АВТОСЕРВИС");
            Console.WriteLine("Добро пожаловать! Управляйте автосервисом и зарабатывайте деньги!");
            Console.WriteLine();
            
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    ShowMainMenu();
                    string? choice = Console.ReadLine();
                    
                    switch (choice)
                    {
                        case "1":
                            ProcessNewClient();
                            break;
                        case "2":
                            ShowWarehouse();
                            break;
                        case "3":
                            ShowShop();
                            break;
                        case "4":
                            ShowBalance();
                            break;
                        case "5":
                            ShowStatistics();
                            break;
                        case "0":
                            isRunning = false;
                            Console.WriteLine("Спасибо за игру! До свидания!");
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                    
                    if (isRunning)
                    {
                        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    static void InitializeGame()
    {
        string dbPath = "CarService.db";
        string connectionString = $"Data Source={dbPath};";
        
        _dbContext = new DatabaseContext(connectionString);
        _dbContext.InitializeDatabase();
        
        _partRepository = new PartRepository(_dbContext);
        var warehouseRepository = new WarehouseRepository(_dbContext);
        var orderRepository = new OrderRepository(_dbContext);
        var clientRepository = new ClientRepository(_dbContext);
        _carRepository = new CarRepository(_dbContext);
        var carServiceRepository = new CarServiceRepository(_dbContext);
        
        _financeService = new FinanceService(carServiceRepository);
        _warehouseService = new WarehouseService(warehouseRepository);
        _orderService = new OrderService(orderRepository, _financeService, _warehouseService, _partRepository);
        _clientService = new ClientService(_partRepository, _carRepository, clientRepository);
        _shopService = new ShopService(_partRepository, _warehouseService, _financeService);
    }
    
    static void InitializeTestData()
    {
        if (_partRepository == null || _warehouseService == null)
            return;
        
        var existingParts = _partRepository.GetAll();
        if (existingParts.Count > 0)
            return;
        
        var parts = new[]
        {
            new Part { Name = "Тормозные колодки", Price = 2500.00m },
            new Part { Name = "Масляный фильтр", Price = 800.00m },
            new Part { Name = "Воздушный фильтр", Price = 600.00m },
            new Part { Name = "Свечи зажигания", Price = 1200.00m },
            new Part { Name = "Аккумулятор", Price = 5000.00m },
            new Part { Name = "Генератор", Price = 8000.00m },
            new Part { Name = "Стартер", Price = 6000.00m },
            new Part { Name = "Радиатор", Price = 4500.00m },
            new Part { Name = "Термостат", Price = 1500.00m },
            new Part { Name = "Ремень ГРМ", Price = 3000.00m }
        };
        
        foreach (var part in parts)
        {
            _partRepository.Add(part);
        }
        
        _warehouseService.AddPart(1, 3);
        _warehouseService.AddPart(2, 5);
        _warehouseService.AddPart(3, 4);
        _warehouseService.AddPart(4, 6);
        _warehouseService.AddPart(5, 2);
        
        Console.WriteLine("Тестовые данные загружены!");
    }
    
    static void ShowMainMenu()
    {
        Console.WriteLine("ГЛАВНОЕ МЕНЮ");
        Console.WriteLine($"Баланс: {_financeService?.GetBalance():F2} руб.");
        Console.WriteLine();
        Console.WriteLine("1. Принять нового клиента");
        Console.WriteLine("2. Показать склад");
        Console.WriteLine("3. Магазин запчастей");
        Console.WriteLine("4. Показать баланс");
        Console.WriteLine("5. Статистика");
        Console.WriteLine("0. Выход");
        Console.Write("Выберите действие: ");
    }
    
    static void ProcessNewClient()
    {
        if (_clientService == null || _orderService == null || _warehouseService == null || _partRepository == null || _carRepository == null)
            return;
        
        Console.Clear();
        Console.WriteLine("НОВЫЙ КЛИЕНТ");
        
        var client = _clientService.GenerateRandomClient();
        var car = _carRepository.GetById(client.CarId);
        var brokenPart = _partRepository.GetById(car.BrokenPartId);
        
        var order = _orderService.CreateOrder(client, car, brokenPart);
        
        Console.WriteLine($"Клиент: {client.Name}");
        Console.WriteLine($"Машина: {car.Model}");
        Console.WriteLine($"Сломано: {brokenPart.Name}");
        Console.WriteLine($"Стоимость ремонта: {order.RepairCost:F2} руб.");
        Console.WriteLine();
        
        var hasPart = _warehouseService.HasPart(brokenPart.Id);
        if (hasPart)
        {
            Console.WriteLine($"✓ На складе есть деталь: {brokenPart.Name}");
        }
        else
        {
            Console.WriteLine($"✗ На складе НЕТ детали: {brokenPart.Name}");
        }
        
        Console.WriteLine();
        Console.WriteLine("1. Принять заказ");
        Console.WriteLine("2. Отказать");
        Console.Write("Ваш выбор: ");
        
        string? choice = Console.ReadLine();
        
        if (choice == "1")
        {
            order.Status = OrderStatus.Accepted;
            _orderService.AcceptOrder(order);
            Console.WriteLine("Заказ принят!");
            
            if (_warehouseService.HasPart(brokenPart.Id))
            {
                var success = _orderService.RepairCar(order);
                if (success)
                {
                    Console.WriteLine($"✓ Ремонт выполнен успешно! Получено {order.RepairCost:F2} руб.");
                }
                else
                {
                    Console.WriteLine("✗ Ошибка при ремонте!");
                }
            }
            else
            {
                Console.WriteLine("✗ Детали нет на складе! Производится замена другой деталью...");
                Console.WriteLine("Клиент недоволен неправильным ремонтом!");
                _orderService.FailOrder(order);
                Console.WriteLine($"Штраф за неправильный ремонт: {order.RepairCost * 2:F2} руб.");
            }
        }
        else if (choice == "2")
        {
            _orderService.RejectOrder(order);
            Console.WriteLine("Заказ отклонен. Штраф за отказ: 500.00 руб.");
        }
        
        _carsProcessed++;
        _warehouseService.ProcessDeliveries(1);
    }
    
    static void ShowWarehouse()
    {
        if (_warehouseService == null || _partRepository == null)
            return;
        
        Console.Clear();
        Console.WriteLine("СКЛАД ЗАПЧАСТЕЙ");
        Console.WriteLine();
        
        var warehouseItems = _warehouseService.GetAvailableParts();
        if (warehouseItems.Count == 0)
        {
            Console.WriteLine("Склад пуст.");
        }
        else
        {
            Console.WriteLine($"{"№",-5} {"Деталь",-30} {"Количество",-15}");
            Console.WriteLine(new string('-', 50));
            
            foreach (var item in warehouseItems)
            {
                var part = _partRepository.GetById(item.PartId);
                Console.WriteLine($"{item.PartId,-5} {part.Name,-30} {item.Quantity,-15}");
            }
        }
        
        var pendingDeliveries = _warehouseService.GetPendingDeliveries();
        if (pendingDeliveries.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Ожидаются поставки:");
            foreach (var delivery in pendingDeliveries)
            {
                var part = _partRepository.GetById(delivery.PartId);
                Console.WriteLine($"  - {part.Name}: {delivery.Quantity} шт. (через {_carsProcessed} машин)");
            }
        }
    }
    
    static void ShowShop()
    {
        if (_shopService == null || _financeService == null || _partRepository == null)
            return;
        
        Console.Clear();
        Console.WriteLine("МАГАЗИН ЗАПЧАСТЕЙ");
        Console.WriteLine($"Баланс: {_financeService.GetBalance():F2} руб.");
        Console.WriteLine();
        
        var parts = _shopService.GetAvailablePartsForPurchase();
        if (parts.Count == 0)
        {
            Console.WriteLine("Нет доступных деталей для покупки.");
            return;
        }
        
        Console.WriteLine($"{"№",-5} {"Деталь",-30} {"Цена",-15}");
        Console.WriteLine(new string('-', 50));
        
        foreach (var part in parts)
        {
            Console.WriteLine($"{part.Id,-5} {part.Name,-30} {part.Price:F2} руб.");
        }
        
        Console.WriteLine();
        Console.Write("Введите номер детали для покупки (0 - отмена): ");
        
        if (int.TryParse(Console.ReadLine(), out int partId) && partId > 0)
        {
            var part = parts.Where(p => p.Id == partId).FirstOrDefault();
            if (part == null)
            {
                Console.WriteLine("Деталь не найдена!");
                return;
            }
            
            Console.Write($"Введите количество (цена за штуку: {part.Price:F2} руб.): ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                var totalCost = part.Price * quantity;
                Console.WriteLine($"Общая стоимость: {totalCost:F2} руб.");
                Console.Write("Подтвердить покупку? (y/n): ");
                
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    if (_shopService.BuyParts(partId, quantity))
                    {
                        Console.WriteLine($"Покупка оформлена! Детали будут доставлены через 2 машины.");
                        Console.WriteLine($"Списано: {totalCost:F2} руб.");
                    }
                    else
                    {
                        Console.WriteLine("Недостаточно средств для покупки!");
                    }
                }
            }
            else
            {
                Console.WriteLine("Неверное количество!");
            }
        }
    }
    
    static void ShowBalance()
    {
        if (_financeService == null)
            return;
        
        Console.Clear();
        Console.WriteLine("БАЛАНС");
        Console.WriteLine($"Текущий баланс: {_financeService.GetBalance():F2} руб.");
    }
    
    static void ShowStatistics()
    {
        if (_orderService == null || _financeService == null)
            return;
        
        Console.Clear();
        Console.WriteLine("СТАТИСТИКА");
        Console.WriteLine($"Баланс: {_financeService.GetBalance():F2} руб.");
        Console.WriteLine($"Обработано машин: {_carsProcessed}");
    }
}
