using System.Linq;
using pr7.Models;
using pr7.Repositories;

namespace pr7.Services;

public class WarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private int _carsProcessed = 0;
    private const int DELIVERY_DELAY_CARS = 2;
    private readonly Dictionary<int, int> _pendingDeliveries = new Dictionary<int, int>();
    
    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }
    
    public bool HasPart(int partId, int quantity = 1)
    {
        if (quantity < 0)
            return false;
        
        var item = _warehouseRepository.GetByPartId(partId);
        if (item == null)
            return false;
        
        if (item.DeliveryDate != null && item.DeliveryDate > DateTime.Now)
            return false;
        
        return item.Quantity >= quantity;
    }
    
    public void AddPart(int partId, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        var item = _warehouseRepository.GetByPartId(partId);
        if (item == null)
        {
            item = new WarehouseItem
            {
                PartId = partId,
                Quantity = quantity,
                DeliveryDate = null
            };
            _warehouseRepository.Add(item);
        }
        else
        {
            item.Quantity += quantity;
            _warehouseRepository.Update(item);
        }
    }
    
    public void RemovePart(int partId, int quantity = 1)
    {
        if (quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        if (!HasPart(partId, quantity))
            throw new InvalidOperationException("Недостаточно деталей на складе");
        
        var item = _warehouseRepository.GetByPartId(partId);
        if (item != null)
        {
            item.Quantity -= quantity;
            if (item.Quantity == 0)
            {
                _warehouseRepository.Delete(item.Id);
            }
            else
            {
                _warehouseRepository.Update(item);
            }
        }
    }
    
    public List<WarehouseItem> GetAvailableParts()
    {
        return _warehouseRepository.GetAll();
    }
    
    public List<WarehouseItem> GetPendingDeliveries()
    {
        return _warehouseRepository.GetPendingDeliveries();
    }
    
    public void ProcessDeliveries(int carsProcessed)
    {
        _carsProcessed += carsProcessed;
        
        var pendingDeliveries = _warehouseRepository.GetPendingDeliveries();
        var deliveriesToProcess = new List<WarehouseItem>();
        
        foreach (var delivery in pendingDeliveries)
        {
            if (!_pendingDeliveries.ContainsKey(delivery.Id))
            {
                _pendingDeliveries[delivery.Id] = DELIVERY_DELAY_CARS;
            }
            
            _pendingDeliveries[delivery.Id] -= carsProcessed;
            
            if (_pendingDeliveries[delivery.Id] <= 0)
            {
                deliveriesToProcess.Add(delivery);
            }
        }
        
        foreach (var delivery in deliveriesToProcess)
        {
            var existingItem = _warehouseRepository.GetByPartId(delivery.PartId);
            if (existingItem == null || existingItem.DeliveryDate != null)
            {
                var newItem = new WarehouseItem
                {
                    PartId = delivery.PartId,
                    Quantity = delivery.Quantity,
                    DeliveryDate = null
                };
                _warehouseRepository.Add(newItem);
            }
            else
            {
                existingItem.Quantity += delivery.Quantity;
                _warehouseRepository.Update(existingItem);
            }
            
            _warehouseRepository.Delete(delivery.Id);
            _pendingDeliveries.Remove(delivery.Id);
        }
    }
    
    public void ScheduleDelivery(int partId, int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        var deliveryDate = DateTime.Now.AddDays(365);
        
        var item = new WarehouseItem
        {
            PartId = partId,
            Quantity = quantity,
            DeliveryDate = deliveryDate
        };
        
        _warehouseRepository.Add(item);
    }
    
    public int GetRandomAvailablePartId()
    {
        var availableParts = GetAvailableParts();
        if (availableParts.Count == 0)
            throw new InvalidOperationException("На складе нет доступных деталей");
        
        var random = new Random();
        var randomIndex = random.Next(availableParts.Count);
        return availableParts[randomIndex].PartId;
    }
}

