using pr8.Models;

namespace pr8.Repositories;

public interface ICartRepository
{
    Cart? GetByUserId(int userId);
    Cart? GetById(int id);
    void Add(Cart cart);
    void Update(Cart cart);
    void Delete(int id);
}

