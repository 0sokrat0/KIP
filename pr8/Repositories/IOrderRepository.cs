using pr8.Models;

namespace pr8.Repositories;

public interface IOrderRepository
{
    Order? GetById(int id);
    List<Order> GetByUserId(int userId);
    void Add(Order order);
    void Update(Order order);
    void Delete(int id);
}

