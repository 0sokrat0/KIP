using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly CartService _cartService;
    
    public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IProductRepository productRepository, CartService cartService)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _cartService = cartService;
    }
    
    public Order CreateOrder(int userId, int pickupPointId, List<CartItem> cartItems)
    {
        if (cartItems.Count == 0)
        {
            throw new ArgumentException("Корзина пуста");
        }
        
        decimal totalAmount = 0;
        foreach (var cartItem in cartItems)
        {
            var product = _productRepository.GetById(cartItem.ProductId);
            if (product != null)
            {
                totalAmount += product.Price * cartItem.Quantity;
            }
        }
        
        var order = new Order
        {
            UserId = userId,
            PickupPointId = pickupPointId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount
        };
        
        _orderRepository.Add(order);
        
        foreach (var cartItem in cartItems)
        {
            var product = _productRepository.GetById(cartItem.ProductId);
            if (product != null)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = product.Price
                };
                _orderItemRepository.Add(orderItem);
            }
        }
        
        _cartService.ClearCart(userId);
        
        return order;
    }
    
    public Order CreateSingleItemOrder(int userId, int productId, int quantity, int pickupPointId)
    {
        var product = _productRepository.GetById(productId);
        if (product == null)
        {
            throw new ArgumentException("Товар не найден");
        }
        
        if (quantity <= 0)
        {
            throw new ArgumentException("Количество должно быть больше нуля");
        }
        
        decimal totalAmount = product.Price * quantity;
        
        var order = new Order
        {
            UserId = userId,
            PickupPointId = pickupPointId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount
        };
        
        _orderRepository.Add(order);
        
        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = productId,
            Quantity = quantity,
            Price = product.Price
        };
        _orderItemRepository.Add(orderItem);
        
        return order;
    }
    
    public List<Order> GetUserOrders(int userId, bool sortAscending = true)
    {
        var orders = _orderRepository.GetByUserId(userId);
        
        if (sortAscending)
        {
            return orders.OrderBy(o => o.OrderDate).ToList();
        }
        else
        {
            return orders.OrderByDescending(o => o.OrderDate).ToList();
        }
    }
    
    public void DisplayOrders(List<Order> orders)
    {
        if (orders.Count == 0)
        {
            Console.WriteLine("Заказы не найдены");
            return;
        }
        
        Console.WriteLine("ИСТОРИЯ ЗАКАЗОВ");
        Console.WriteLine(new string('-', 100));
        Console.WriteLine($"{"ID",-5} {"Дата",-20} {"ПВЗ ID",-10} {"Сумма",-15}");
        Console.WriteLine(new string('-', 100));
        
        foreach (var order in orders)
        {
            Console.WriteLine($"{order.Id,-5} {order.OrderDate:yyyy-MM-dd HH:mm:ss,-20} {order.PickupPointId,-10} {order.TotalAmount,-15:F2} руб.");
        }
        
        Console.WriteLine(new string('-', 100));
    }
    
    public List<OrderItem> GetOrderItems(int orderId)
    {
        return _orderItemRepository.GetByOrderId(orderId);
    }
}
