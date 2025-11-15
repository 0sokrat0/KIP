using pr7.Models;
using pr7.Repositories;

namespace pr7.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IFinanceService _financeService;
    private readonly IWarehouseService _warehouseService;
    private readonly IPartRepository _partRepository;
    private const decimal FAILURE_PENALTY_MULTIPLIER = 2.0m;
    
    public OrderService(
        IOrderRepository orderRepository,
        IFinanceService financeService,
        IWarehouseService warehouseService,
        IPartRepository partRepository)
    {
        _orderRepository = orderRepository;
        _financeService = financeService;
        _warehouseService = warehouseService;
        _partRepository = partRepository;
    }
    
    public Order CreateOrder(Client client, Car car, Part brokenPart)
    {
        var repairCost = CalculateRepairCost(brokenPart);
        
        var order = new Order
        {
            ClientId = client.Id,
            CarId = car.Id,
            BrokenPartId = brokenPart.Id,
            RepairCost = repairCost,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now
        };
        
        _orderRepository.Add(order);
        return order;
    }
    
    public bool AcceptOrder(Order order)
    {
        if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Accepted)
        {
            order.Status = OrderStatus.Accepted;
            _orderRepository.Update(order);
            return true;
        }
        
        return false;
    }
    
    public void RejectOrder(Order order)
    {
        if (order.Status != OrderStatus.Pending)
            return;
        
        order.Status = OrderStatus.Rejected;
        _orderRepository.Update(order);
        _financeService.PayPenalty();
    }
    
    public bool RepairCar(Order order)
    {
        if (order.Status != OrderStatus.Accepted)
            return false;
        
        try
        {
            if (_warehouseService.HasPart(order.BrokenPartId))
            {
                _warehouseService.RemovePart(order.BrokenPartId);
                _financeService.AddMoney(order.RepairCost);
                CompleteOrder(order);
                return true;
            }
            else
            {
                FailOrder(order);
                return false;
            }
        }
        catch
        {
            FailOrder(order);
            return false;
        }
    }
    
    public void CompleteOrder(Order order)
    {
        order.Status = OrderStatus.Completed;
        _orderRepository.Update(order);
    }
    
    public void FailOrder(Order order)
    {
        order.Status = OrderStatus.Failed;
        _orderRepository.Update(order);
        
        var penalty = order.RepairCost * FAILURE_PENALTY_MULTIPLIER;
        _financeService.PayPenalty(penalty);
        
        try
        {
            var randomPartId = _warehouseService.GetRandomAvailablePartId();
            _warehouseService.RemovePart(randomPartId);
        }
        catch
        {
        }
    }
    
    public decimal CalculateRepairCost(Part part)
    {
        var workCost = _financeService.CalculateWorkCost(part.Price);
        return part.Price + workCost;
    }
}

