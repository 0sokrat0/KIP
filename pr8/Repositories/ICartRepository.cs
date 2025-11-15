using pr8.Models;

namespace pr8.Repositories;

public interface ICartRepository
{
    List<CartItem> GetByUserId(int userId);
    CartItem? GetByUserAndProduct(int userId, int productId);
    void Add(CartItem item);
    void Update(CartItem item);
    void Delete(int id);
    void ClearUserCart(int userId);
}

