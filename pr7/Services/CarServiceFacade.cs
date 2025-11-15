using System;
using System.Collections.Generic;
using System.Linq;
using pr7.Models;
using pr7.Repositories;

namespace pr7.Services;

public class CarServiceFacade : ICarServiceFacade
{
    private readonly IClientService _clientService;
    private readonly IOrderService _orderService;
    private readonly IWarehouseService _warehouseService;
    private readonly IShopService _shopService;
    private readonly IFinanceService _financeService;
    private readonly IPartRepository _partRepository;
    private readonly ICarRepository _carRepository;
    
    private Order? _currentOrder;
    private int _carsProcessed = 0;
    
    public bool HasPendingOrder { get; private set; }
    public Order? CurrentOrder => _currentOrder;
    
    public event EventHandler<string[]>? OutputChanged;
    public event EventHandler<string>? BalanceChanged;
    
    public CarServiceFacade(
        IClientService clientService,
        IOrderService orderService,
        IWarehouseService warehouseService,
        IShopService shopService,
        IFinanceService financeService,
        IPartRepository partRepository,
        ICarRepository carRepository)
    {
        _clientService = clientService;
        _orderService = orderService;
        _warehouseService = warehouseService;
        _shopService = shopService;
        _financeService = financeService;
        _partRepository = partRepository;
        _carRepository = carRepository;
    }
    
    public void ProcessNewClient()
    {
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
        
        HasPendingOrder = true;
        OutputChanged?.Invoke(this, lines.ToArray());
    }
    
    public void AcceptOrder()
    {
        if (_currentOrder == null)
            return;
            
        var brokenPart = _partRepository.GetById(_currentOrder.BrokenPartId);
        var hasPart = _warehouseService.HasPart(brokenPart.Id);
        
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
            _orderService.FailOrder(_currentOrder);
            var penalty = _currentOrder.RepairCost * 2;
            lines.Add($"Штраф: {penalty:F2} руб.");
        }
        
        _carsProcessed++;
        _warehouseService.ProcessDeliveries(1);
        HasPendingOrder = false;
        _currentOrder = null;
        
        OutputChanged?.Invoke(this, lines.ToArray());
        BalanceChanged?.Invoke(this, $"Баланс: {_financeService.GetBalance():F2} руб.");
    }
    
    public void RejectOrder()
    {
        if (_currentOrder == null)
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
        _warehouseService.ProcessDeliveries(1);
        HasPendingOrder = false;
        _currentOrder = null;
        
        OutputChanged?.Invoke(this, lines.ToArray());
        BalanceChanged?.Invoke(this, $"Баланс: {_financeService.GetBalance():F2} руб.");
    }
    
    public void ShowWarehouse()
    {
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
        
        OutputChanged?.Invoke(this, lines.ToArray());
    }
    
    public void ShowShop()
    {
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
            OutputChanged?.Invoke(this, lines.ToArray());
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
        OutputChanged?.Invoke(this, lines.ToArray());
    }
    
    public void BuyPart(int partId, int quantity)
    {
        var parts = _shopService.GetAvailablePartsForPurchase();
        var part = parts.FirstOrDefault(p => p.Id == partId);
        if (part == null)
        {
            OutputChanged?.Invoke(this, new[] { $"Ошибка: Деталь с ID {partId} не найдена!" });
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
        }
        else
        {
            lines.Add("Недостаточно средств для покупки!");
            lines.Add($"Нужно: {totalCost:F2} руб., доступно: {_financeService.GetBalance():F2} руб.");
        }
        
        OutputChanged?.Invoke(this, lines.ToArray());
        BalanceChanged?.Invoke(this, $"Баланс: {_financeService.GetBalance():F2} руб.");
    }
    
    public decimal GetBalance()
    {
        return _financeService.GetBalance();
    }
    
    public int GetCarsProcessed()
    {
        return _carsProcessed;
    }
    
    public void ShowBalance()
    {
        var lines = new[]
        {
            "БАЛАНС",
            "",
            $"Текущий баланс: {_financeService.GetBalance():F2} руб."
        };
        OutputChanged?.Invoke(this, lines);
        BalanceChanged?.Invoke(this, $"Баланс: {_financeService.GetBalance():F2} руб.");
    }
    
    public void ShowStatistics()
    {
        var lines = new[]
        {
            "СТАТИСТИКА",
            "",
            $"Баланс: {_financeService.GetBalance():F2} руб.",
            $"Обработано машин: {_carsProcessed}"
        };
        OutputChanged?.Invoke(this, lines);
    }
}

