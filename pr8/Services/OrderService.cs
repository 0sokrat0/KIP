using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    
    public OrderService(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _cartRepository = cartRepository;
    }
    
    public Order CreateOrder(int userId, int pickupPointId, List<CartItem> cartItems)
    {
        if (cartItems == null || cartItems.Count == 0)
            throw new ArgumentException("Корзина пуста");
        
        decimal totalAmount = 0;
        
        foreach (var cartItem in cartItems)
        {
            var product = _productRepository.GetById(cartItem.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Товар с Id {cartItem.ProductId} не найден");
            
            if (product.Stock < cartItem.Quantity)
                throw new InvalidOperationException($"Недостаточно товара '{product.Name}' на складе");
            
            totalAmount += product.Price * cartItem.Quantity;
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
            var product = _productRepository.GetById(cartItem.ProductId)!;
            
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = product.Price
            };
            
            _orderItemRepository.Add(orderItem);
            
            product.Stock -= cartItem.Quantity;
            _productRepository.Update(product);
        }
        
        _cartRepository.ClearUserCart(userId);
        
        return order;
    }
    
    public Order CreateSingleItemOrder(int userId, int productId, int quantity, int pickupPointId)
    {
        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        var product = _productRepository.GetById(productId);
        if (product == null)
            throw new KeyNotFoundException("Товар не найден");
        
        if (product.Stock < quantity)
            throw new InvalidOperationException("Недостаточно товара на складе");
        
        var order = new Order
        {
            UserId = userId,
            PickupPointId = pickupPointId,
            OrderDate = DateTime.Now,
            TotalAmount = product.Price * quantity
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
        
        product.Stock -= quantity;
        _productRepository.Update(product);
        
        return order;
    }
    
    public List<Order> GetUserOrders(int userId, bool ascending = true)
    {
        return _orderRepository.GetByUserId(userId, ascending);
    }
    
    public List<OrderItem> GetOrderItems(int orderId)
    {
        return _orderItemRepository.GetByOrderId(orderId);
    }
}

