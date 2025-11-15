using pr7.Models;

namespace pr7.Services;

public interface IOrderService
{
    Order CreateOrder(Client client, Car car, Part brokenPart);
    bool AcceptOrder(Order order);
    void RejectOrder(Order order);
    bool RepairCar(Order order);
    void CompleteOrder(Order order);
    void FailOrder(Order order);
    decimal CalculateRepairCost(Part part);
}

