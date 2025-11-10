using pr7.Models;

namespace pr7.Repositories;

public interface IOrderRepository
{
    Order GetById(int id);
    List<Order> GetAll();
    void Add(Order order);
    void Update(Order order);
    void Delete(int id);
}

