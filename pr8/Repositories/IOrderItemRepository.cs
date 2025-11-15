using pr8.Models;

namespace pr8.Repositories;

public interface IOrderItemRepository
{
    List<OrderItem> GetByOrderId(int orderId);
    void Add(OrderItem item);
}

