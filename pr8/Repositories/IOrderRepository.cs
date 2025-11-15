using pr8.Models;

namespace pr8.Repositories;

public interface IOrderRepository
{
    Order? GetById(int id);
    List<Order> GetByUserId(int userId, bool ascending = true);
    void Add(Order order);
}

