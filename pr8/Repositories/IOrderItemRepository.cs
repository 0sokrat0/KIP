using pr8.Models;

namespace pr8.Repositories;

public interface IOrderItemRepository
{
    OrderItem? GetById(int id);
    List<OrderItem> GetByOrderId(int orderId);
    void Add(OrderItem orderItem);
    void Update(OrderItem orderItem);
    void Delete(int id);
}

