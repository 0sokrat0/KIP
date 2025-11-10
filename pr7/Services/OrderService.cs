using pr7.Models;

namespace pr7.Services;

public class OrderService
{
    public Order CreateOrder(Client client, Car car, Part brokenPart) { return new Order(); }
    
    public bool AcceptOrder(Order order) { return false; }
    
    public void RejectOrder(Order order) { }
    
    public bool RepairCar(Order order) { return false; }
    
    public void CompleteOrder(Order order) { }
    
    public void FailOrder(Order order) { }
    
    public decimal CalculateRepairCost(Part part) { return 0; }
}

