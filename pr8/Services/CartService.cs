using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    
    public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
    }
    
    public void AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            Console.WriteLine("Количество должно быть больше нуля");
            return;
        }
        
        var product = _productRepository.GetById(productId);
        if (product == null)
        {
            Console.WriteLine("Товар не найден");
            return;
        }
        
        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _cartRepository.Add(cart);
        }
        
        var existingItem = _cartItemRepository.GetByCartIdAndProductId(cart.Id, productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            _cartItemRepository.Update(existingItem);
            Console.WriteLine($"Товар добавлен в корзину. Всего в корзине: {existingItem.Quantity}");
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };
            _cartItemRepository.Add(cartItem);
            Console.WriteLine($"Товар добавлен в корзину");
        }
    }
    
    public List<CartItem> GetCartItems(int userId)
    {
        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            return new List<CartItem>();
        }
        
        return _cartItemRepository.GetByCartId(cart.Id);
    }
    
    public void DisplayCart(int userId)
    {
        var cartItems = GetCartItems(userId);
        
        if (cartItems.Count == 0)
        {
            Console.WriteLine("Корзина пуста");
            return;
        }
        
        Console.WriteLine("КОРЗИНА");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"ID",-5} {"Товар",-30} {"Количество",-15} {"Цена",-15} {"Сумма"}");
        Console.WriteLine(new string('-', 80));
        
        decimal total = 0;
        foreach (var item in cartItems)
        {
            var product = _productRepository.GetById(item.ProductId);
            if (product != null)
            {
                decimal itemTotal = product.Price * item.Quantity;
                total += itemTotal;
                Console.WriteLine($"{item.Id,-5} {product.Name,-30} {item.Quantity,-15} {product.Price,-15:F2} руб. {itemTotal:F2} руб.");
            }
        }
        
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"ИТОГО: {total:F2} руб.");
    }
    
    public void RemoveFromCart(int userId, int productId)
    {
        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            Console.WriteLine("Корзина не найдена");
            return;
        }
        
        var cartItem = _cartItemRepository.GetByCartIdAndProductId(cart.Id, productId);
        if (cartItem == null)
        {
            Console.WriteLine("Товар не найден в корзине");
            return;
        }
        
        _cartItemRepository.Delete(cartItem.Id);
        Console.WriteLine("Товар удален из корзины");
    }
    
    public void ClearCart(int userId)
    {
        var cart = _cartRepository.GetByUserId(userId);
        if (cart == null)
        {
            return;
        }
        
        _cartItemRepository.DeleteByCartId(cart.Id);
    }
    
    public decimal CalculateCartTotal(int userId)
    {
        var cartItems = GetCartItems(userId);
        decimal total = 0;
        
        foreach (var item in cartItems)
        {
            var product = _productRepository.GetById(item.ProductId);
            if (product != null)
            {
                total += product.Price * item.Quantity;
            }
        }
        
        return total;
    }
}
