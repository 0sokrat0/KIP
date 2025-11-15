using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using pr7.Data;
using pr7.Models;
using pr7.Repositories;
using pr7.Services;

namespace pr7.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _balanceText = "Баланс: 0.00 руб.";
    private string _shopPartId = "";
    private string _shopQuantity = "";
    private Order? _currentOrder;
    private bool _hasPendingOrder = false;
    
    private ObservableCollection<string> _outputLines = new();
    
    private DatabaseContext? _dbContext;
    private FinanceService? _financeService;
    private WarehouseService? _warehouseService;
    private OrderService? _orderService;
    private ClientService? _clientService;
    private ShopService? _shopService;
    private IPartRepository? _partRepository;
    private ICarRepository? _carRepository;
    private int _carsProcessed = 0;

    public ObservableCollection<string> OutputLines
    {
        get => _outputLines;
        set => SetProperty(ref _outputLines, value);
    }

    public string BalanceText
    {
        get => _balanceText;
        set => SetProperty(ref _balanceText, value);
    }

    public string ShopPartId
    {
        get => _shopPartId;
        set => SetProperty(ref _shopPartId, value);
    }

    public string ShopQuantity
    {
        get => _shopQuantity;
        set => SetProperty(ref _shopQuantity, value);
    }

    public bool HasPendingOrder
    {
        get => _hasPendingOrder;
        set
        {
            if (SetProperty(ref _hasPendingOrder, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public ICommand ProcessNewClientCommand { get; }
    public ICommand AcceptOrderCommand { get; }
    public ICommand RejectOrderCommand { get; }
    public ICommand ShowWarehouseCommand { get; }
    public ICommand ShowShopCommand { get; }
    public ICommand BuyPartCommand { get; }
    public ICommand ShowBalanceCommand { get; }
    public ICommand ShowStatisticsCommand { get; }
    public ICommand ExitCommand { get; }

    public MainWindowViewModel()
    {
        ProcessNewClientCommand = new RelayCommand(ProcessNewClient);
        AcceptOrderCommand = new RelayCommand(AcceptOrder, () => HasPendingOrder);
        RejectOrderCommand = new RelayCommand(RejectOrder, () => HasPendingOrder);
        ShowWarehouseCommand = new RelayCommand(ShowWarehouse);
        ShowShopCommand = new RelayCommand(ShowShop);
        BuyPartCommand = new RelayCommand(BuyPart);
        ShowBalanceCommand = new RelayCommand(ShowBalance);
        ShowStatisticsCommand = new RelayCommand(ShowStatistics);
        ExitCommand = new RelayCommand(Exit);
        
        InitializeGame();
        InitializeTestData();
        UpdateBalance();
        
        ShowWelcomeMessage();
    }
    
    private void ShowWelcomeMessage()
    {
        OutputLines.Clear();
        OutputLines.Add("Добро пожаловать! Управляйте автосервисом и зарабатывайте деньги!");
        OutputLines.Add("");
        OutputLines.Add("Нажмите 'Принять нового клиента' для начала работы.");
    }
    
    private void SetOutputLines(params string[] lines)
    {
        OutputLines.Clear();
        foreach (var line in lines)
        {
            OutputLines.Add(line);
        }
    }

    private void InitializeGame()
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

    private void InitializeTestData()
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
    }

    private void UpdateBalance()
    {
        if (_financeService != null)
        {
            BalanceText = $"Баланс: {_financeService.GetBalance():F2} руб.";
        }
    }

    private void ProcessNewClient()
    {
        try
        {
            if (_clientService == null || _orderService == null || _warehouseService == null || _partRepository == null || _carRepository == null)
                return;
            
            var client = _clientService.GenerateRandomClient();
            var car = _carRepository.GetById(client.CarId);
            var brokenPart = _partRepository.GetById(car.BrokenPartId);
            
            _currentOrder = _orderService.CreateOrder(client, car, brokenPart);
            
            var hasPart = _warehouseService.HasPart(brokenPart.Id);
            
            var lines = new List<string>
            {
                "НОВЫЙ КЛИЕНТ",
                new string('=', 50),
                "",
                $"Клиент: {client.Name}",
                $"Машина: {car.Model}",
                $"Сломано: {brokenPart.Name}",
                $"Стоимость ремонта: {_currentOrder.RepairCost:F2} руб.",
                ""
            };
            
            if (hasPart)
            {
                lines.Add("На складе ЕСТЬ деталь: " + brokenPart.Name);
                lines.Add("Количество на складе: " + _warehouseService.GetAvailableParts().First(w => w.PartId == brokenPart.Id).Quantity + " шт.");
                lines.Add("");
            }
            else
            {
                lines.Add("На складе НЕТ детали: " + brokenPart.Name);
                lines.Add("ВНИМАНИЕ: Если примете заказ без детали, будет штраф!");
                lines.Add("");
            }
            
            lines.Add("Выберите действие:");
            lines.Add("- Принять заказ - получите деньги, если деталь есть");
            lines.Add("- Отказать - заплатите штраф 500 руб.");
            
            SetOutputLines(lines.ToArray());
            HasPendingOrder = true;
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
            HasPendingOrder = false;
        }
    }

    private void AcceptOrder()
    {
        try
        {
            if (_currentOrder == null || _orderService == null || _warehouseService == null || _partRepository == null || _carRepository == null)
                return;

            var brokenPart = _partRepository.GetById(_currentOrder.BrokenPartId);
            var hasPart = _warehouseService.HasPart(brokenPart.Id);
            
            _currentOrder.Status = OrderStatus.Accepted;
            _orderService.AcceptOrder(_currentOrder);
            
            var lines = new List<string> { "ЗАКАЗ ПРИНЯТ", "" };
            
            if (hasPart)
            {
                var success = _orderService.RepairCar(_currentOrder);
                if (success)
                {
                    lines.Add("Ремонт выполнен успешно!");
                    lines.Add($"Получено: {_currentOrder.RepairCost:F2} руб.");
                }
                else
                {
                    lines.Add("Ошибка при ремонте!");
                }
            }
            else
            {
                lines.Add("ВНИМАНИЕ: Детали нет на складе!");
                lines.Add("Производится замена другой деталью...");
                lines.Add("Клиент недоволен неправильным ремонтом!");
                try
                {
                    _orderService.FailOrder(_currentOrder);
                    var penalty = _currentOrder.RepairCost * 2;
                    lines.Add($"Штраф: {penalty:F2} руб.");
                }
                catch (Exception ex)
                {
                    lines.Add($"Ошибка: {ex.Message}");
                }
            }
            
            _carsProcessed++;
            _warehouseService.ProcessDeliveries(1);
            HasPendingOrder = false;
            _currentOrder = null;
            SetOutputLines(lines.ToArray());
            UpdateBalance();
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
            HasPendingOrder = false;
        }
    }

    private void RejectOrder()
    {
        try
        {
            if (_currentOrder == null || _orderService == null)
                return;

            _orderService.RejectOrder(_currentOrder);
            
            var lines = new List<string>
            {
                "ЗАКАЗ ОТКЛОНЕН",
                "",
                "Вы отказали клиенту",
                "Штраф за отказ: 500.00 руб."
            };
            
            _carsProcessed++;
            if (_warehouseService != null)
            {
                _warehouseService.ProcessDeliveries(1);
            }
            
            HasPendingOrder = false;
            _currentOrder = null;
            SetOutputLines(lines.ToArray());
            UpdateBalance();
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
            HasPendingOrder = false;
        }
    }

    private void ShowWarehouse()
    {
        try
        {
            if (_warehouseService == null || _partRepository == null)
                return;
            
            var lines = new List<string> { "СКЛАД ЗАПЧАСТЕЙ", "" };
            
            var warehouseItems = _warehouseService.GetAvailableParts();
            if (warehouseItems.Count == 0)
            {
                lines.Add("Склад пуст.");
            }
            else
            {
                lines.Add($"{"№",-5} {"Деталь",-35} {"Количество",-15}");
                lines.Add(new string('-', 55));
                
                foreach (var item in warehouseItems)
                {
                    var part = _partRepository.GetById(item.PartId);
                    lines.Add($"{item.PartId,-5} {part.Name,-35} {item.Quantity,-15} шт.");
                }
            }
            
            var pendingDeliveries = _warehouseService.GetPendingDeliveries();
            if (pendingDeliveries.Count > 0)
            {
                lines.Add("");
                lines.Add("Ожидаются поставки:");
                foreach (var delivery in pendingDeliveries)
                {
                    var part = _partRepository.GetById(delivery.PartId);
                    lines.Add($"  - {part.Name}: {delivery.Quantity} шт.");
                }
            }
            
            SetOutputLines(lines.ToArray());
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
        }
    }

    private void ShowShop()
    {
        try
        {
            if (_shopService == null || _financeService == null || _partRepository == null)
                return;
            
            var lines = new List<string>
            {
                "МАГАЗИН ЗАПЧАСТЕЙ",
                "",
                $"Баланс: {_financeService.GetBalance():F2} руб.",
                ""
            };
            
            var parts = _shopService.GetAvailablePartsForPurchase();
            if (parts.Count == 0)
            {
                lines.Add("Нет доступных деталей для покупки.");
                SetOutputLines(lines.ToArray());
                return;
            }
            
            lines.Add($"{"№",-5} {"Деталь",-35} {"Цена",-15}");
            lines.Add(new string('-', 55));
            
            foreach (var part in parts)
            {
                lines.Add($"{part.Id,-5} {part.Name,-35} {part.Price:F2} руб.");
            }
            
            lines.Add("");
            lines.Add("Для покупки введите ID детали и количество в полях ниже.");
            SetOutputLines(lines.ToArray());
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
        }
    }

    private void ShowBalance()
    {
        try
        {
            if (_financeService == null)
                return;
            
            SetOutputLines(
                "БАЛАНС",
                "",
                $"Текущий баланс: {_financeService.GetBalance():F2} руб."
            );
            UpdateBalance();
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
        }
    }

    private void BuyPart()
    {
        try
        {
            if (_shopService == null || _financeService == null || _partRepository == null)
                return;

            if (!int.TryParse(ShopPartId, out int partId) || partId <= 0)
            {
                SetOutputLines("Ошибка: Введите корректный ID детали (положительное число)");
                return;
            }

            if (!int.TryParse(ShopQuantity, out int quantity) || quantity <= 0)
            {
                SetOutputLines("Ошибка: Введите корректное количество (положительное число)");
                return;
            }

            var parts = _shopService.GetAvailablePartsForPurchase();
            var part = parts.FirstOrDefault(p => p.Id == partId);
            if (part == null)
            {
                SetOutputLines($"Ошибка: Деталь с ID {partId} не найдена!");
                return;
            }

            var totalCost = part.Price * quantity;
            var lines = new List<string>
            {
                "ПОКУПКА ЗАПЧАСТИ",
                "",
                $"Деталь: {part.Name}",
                $"Количество: {quantity} шт.",
                $"Цена за штуку: {part.Price:F2} руб.",
                $"Общая стоимость: {totalCost:F2} руб.",
                $"Текущий баланс: {_financeService.GetBalance():F2} руб.",
                ""
            };

            if (_shopService.BuyParts(partId, quantity))
            {
                lines.Add("Покупка оформлена!");
                lines.Add("Детали будут доставлены через 2 машины.");
                lines.Add($"Списано: {totalCost:F2} руб.");
                ShopPartId = "";
                ShopQuantity = "";
            }
            else
            {
                lines.Add("Недостаточно средств для покупки!");
                lines.Add($"Нужно: {totalCost:F2} руб., доступно: {_financeService.GetBalance():F2} руб.");
            }

            SetOutputLines(lines.ToArray());
            UpdateBalance();
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
        }
    }

    private void ShowStatistics()
    {
        try
        {
            if (_orderService == null || _financeService == null)
                return;
            
            SetOutputLines(
                "СТАТИСТИКА",
                "",
                $"Баланс: {_financeService.GetBalance():F2} руб.",
                $"Обработано машин: {_carsProcessed}"
            );
        }
        catch (Exception ex)
        {
            SetOutputLines($"Ошибка: {ex.Message}");
        }
    }

    private void Exit()
    {
        Environment.Exit(0);
    }
}

