using pr8.Models;

namespace pr8.Repositories;

public interface ICartItemRepository
{
    CartItem? GetById(int id);
    List<CartItem> GetByCartId(int cartId);
    CartItem? GetByCartIdAndProductId(int cartId, int productId);
    void Add(CartItem cartItem);
    void Update(CartItem cartItem);
    void Delete(int id);
    void DeleteByCartId(int cartId);
}

