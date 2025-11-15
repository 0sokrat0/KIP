using pr8.Models;

namespace pr8.Services;

public interface IMarketplaceFacade
{
    bool Register(string username, string password, string confirmPassword);
    User? Login(string username, string password);
    void Logout();
    User? CurrentUser { get; }
    bool IsLoggedIn { get; }
    
    List<Product> GetAllProducts();
    Product? GetProductById(int id);
    
    List<CartItem> GetCartItems();
    void AddToCart(int productId, int quantity);
    void UpdateCartItem(int productId, int quantity);
    void RemoveFromCart(int productId);
    void ClearCart();
    decimal GetCartTotal();
    
    List<PickupPoint> GetAllPickupPoints();
    
    Order CreateOrder(int pickupPointId);
    Order CreateSingleItemOrder(int productId, int quantity, int pickupPointId);
    List<Order> GetUserOrders(bool ascending = true);
    List<OrderItem> GetOrderItems(int orderId);
    
    event EventHandler<string[]>? OutputChanged;
    event EventHandler? UserChanged;
}

