using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    
    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }
    
    public List<CartItem> GetCartItems(int userId)
    {
        return _cartRepository.GetByUserId(userId);
    }
    
    public void AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть больше нуля");
        
        var product = _productRepository.GetById(productId);
        if (product == null)
            throw new KeyNotFoundException("Товар не найден");
        
        if (product.Stock < quantity)
            throw new InvalidOperationException("Недостаточно товара на складе");
        
        var existingItem = _cartRepository.GetByUserAndProduct(userId, productId);
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (product.Stock < newQuantity)
                throw new InvalidOperationException("Недостаточно товара на складе");
            
            existingItem.Quantity = newQuantity;
            _cartRepository.Update(existingItem);
        }
        else
        {
            var item = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };
            _cartRepository.Add(item);
        }
    }
    
    public void UpdateCartItem(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            RemoveFromCart(userId, productId);
            return;
        }
        
        var product = _productRepository.GetById(productId);
        if (product == null)
            throw new KeyNotFoundException("Товар не найден");
        
        if (product.Stock < quantity)
            throw new InvalidOperationException("Недостаточно товара на складе");
        
        var item = _cartRepository.GetByUserAndProduct(userId, productId);
        if (item == null)
            throw new KeyNotFoundException("Товар не найден в корзине");
        
        item.Quantity = quantity;
        _cartRepository.Update(item);
    }
    
    public void RemoveFromCart(int userId, int productId)
    {
        var item = _cartRepository.GetByUserAndProduct(userId, productId);
        if (item != null)
        {
            _cartRepository.Delete(item.Id);
        }
    }
    
    public void ClearCart(int userId)
    {
        _cartRepository.ClearUserCart(userId);
    }
    
    public decimal GetCartTotal(int userId)
    {
        var items = _cartRepository.GetByUserId(userId);
        decimal total = 0;
        
        foreach (var item in items)
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

