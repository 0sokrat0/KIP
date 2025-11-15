using pr8.Models;

namespace pr8.Services;

public interface IOrderService
{
    Order CreateOrder(int userId, int pickupPointId, List<CartItem> cartItems);
    Order CreateSingleItemOrder(int userId, int productId, int quantity, int pickupPointId);
    List<Order> GetUserOrders(int userId, bool ascending = true);
    List<OrderItem> GetOrderItems(int orderId);
}

