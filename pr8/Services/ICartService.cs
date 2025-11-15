using pr8.Models;

namespace pr8.Services;

public interface ICartService
{
    List<CartItem> GetCartItems(int userId);
    void AddToCart(int userId, int productId, int quantity);
    void UpdateCartItem(int userId, int productId, int quantity);
    void RemoveFromCart(int userId, int productId);
    void ClearCart(int userId);
    decimal GetCartTotal(int userId);
}

