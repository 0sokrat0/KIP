using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class MarketplaceFacade : IMarketplaceFacade
{
    private readonly IAuthService _authService;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IPickupPointService _pickupPointService;
    private readonly IProductRepository _productRepository;
    
    private User? _currentUser;
    
    public User? CurrentUser => _currentUser;
    public bool IsLoggedIn => _currentUser != null;
    
    public event EventHandler<string[]>? OutputChanged;
    public event EventHandler? UserChanged;
    
    public MarketplaceFacade(
        IAuthService authService,
        IProductService productService,
        ICartService cartService,
        IOrderService orderService,
        IPickupPointService pickupPointService,
        IProductRepository productRepository)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _pickupPointService = pickupPointService;
        _productRepository = productRepository;
    }
    
    public bool Register(string username, string password, string confirmPassword)
    {
        try
        {
            _authService.Register(username, password, confirmPassword);
            var lines = new[]
            {
                "РЕГИСТРАЦИЯ УСПЕШНА",
                "",
                $"Пользователь '{username}' успешно зарегистрирован.",
                "Теперь вы можете войти в систему."
            };
            OutputChanged?.Invoke(this, lines);
            return true;
        }
        catch (Exception ex)
        {
            var lines = new[] { $"Ошибка регистрации: {ex.Message}" };
            OutputChanged?.Invoke(this, lines);
            return false;
        }
    }
    
    public User? Login(string username, string password)
    {
        try
        {
            var user = _authService.Login(username, password);
            if (user != null)
            {
                _currentUser = user;
                var lines = new[]
                {
                    "ВХОД ВЫПОЛНЕН",
                    "",
                    $"Добро пожаловать, {user.Username}!"
                };
                OutputChanged?.Invoke(this, lines);
                UserChanged?.Invoke(this, EventArgs.Empty);
                return user;
            }
            else
            {
                var lines = new[] { "Ошибка: Неверное имя пользователя или пароль." };
                OutputChanged?.Invoke(this, lines);
                return null;
            }
        }
        catch (Exception ex)
        {
            var lines = new[] { $"Ошибка входа: {ex.Message}" };
            OutputChanged?.Invoke(this, lines);
            return null;
        }
    }
    
    public void Logout()
    {
        _currentUser = null;
        var lines = new[] { "Вы вышли из системы." };
        OutputChanged?.Invoke(this, lines);
        UserChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public List<Product> GetAllProducts()
    {
        return _productService.GetAllProducts();
    }
    
    public Product? GetProductById(int id)
    {
        return _productService.GetProductById(id);
    }
    
    public List<CartItem> GetCartItems()
    {
        if (!IsLoggedIn)
            return new List<CartItem>();
        
        return _cartService.GetCartItems(_currentUser!.Id);
    }
    
    public void AddToCart(int productId, int quantity)
    {
        if (!IsLoggedIn)
        {
            var lines = new[] { "Ошибка: Необходимо войти в систему для добавления товара в корзину." };
            OutputChanged?.Invoke(this, lines);
            return;
        }
        
        try
        {
            _cartService.AddToCart(_currentUser!.Id, productId, quantity);
            var product = _productRepository.GetById(productId);
            var lines = new[]
            {
                "ТОВАР ДОБАВЛЕН В КОРЗИНУ",
                "",
                $"Товар: {product?.Name ?? "Неизвестно"}",
                $"Количество: {quantity} шт."
            };
            OutputChanged?.Invoke(this, lines);
        }
        catch (Exception ex)
        {
            var lines = new[] { $"Ошибка: {ex.Message}" };
            OutputChanged?.Invoke(this, lines);
        }
    }
    
    public void UpdateCartItem(int productId, int quantity)
    {
        if (!IsLoggedIn)
            return;
        
        try
        {
            _cartService.UpdateCartItem(_currentUser!.Id, productId, quantity);
        }
        catch (Exception ex)
        {
            var lines = new[] { $"Ошибка: {ex.Message}" };
            OutputChanged?.Invoke(this, lines);
        }
    }
    
    public void RemoveFromCart(int productId)
    {
        if (!IsLoggedIn)
            return;
        
        try
        {
            _cartService.RemoveFromCart(_currentUser!.Id, productId);
        }
        catch (Exception ex)
        {
            var lines = new[] { $"Ошибка: {ex.Message}" };
            OutputChanged?.Invoke(this, lines);
        }
    }
    
    public void ClearCart()
    {
        if (!IsLoggedIn)
            return;
        
        _cartService.ClearCart(_currentUser!.Id);
    }
    
    public decimal GetCartTotal()
    {
        if (!IsLoggedIn)
            return 0;
        
        return _cartService.GetCartTotal(_currentUser!.Id);
    }
    
    public List<PickupPoint> GetAllPickupPoints()
    {
        return _pickupPointService.GetAllPickupPoints();
    }
    
    public Order CreateOrder(int pickupPointId)
    {
        if (!IsLoggedIn)
            throw new InvalidOperationException("Необходимо войти в систему");
        
        var cartItems = _cartService.GetCartItems(_currentUser!.Id);
        if (cartItems.Count == 0)
            throw new InvalidOperationException("Корзина пуста");
        
        var order = _orderService.CreateOrder(_currentUser.Id, pickupPointId, cartItems);
        
        var lines = new[]
        {
            "ЗАКАЗ ОФОРМЛЕН",
            "",
            $"Номер заказа: {order.Id}",
            $"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}",
            $"Сумма: {order.TotalAmount:F2} руб."
        };
        OutputChanged?.Invoke(this, lines);
        
        return order;
    }
    
    public Order CreateSingleItemOrder(int productId, int quantity, int pickupPointId)
    {
        if (!IsLoggedIn)
            throw new InvalidOperationException("Необходимо войти в систему");
        
        var order = _orderService.CreateSingleItemOrder(_currentUser!.Id, productId, quantity, pickupPointId);
        
        var product = _productRepository.GetById(productId);
        var lines = new[]
        {
            "ЗАКАЗ ОФОРМЛЕН",
            "",
            $"Товар: {product?.Name ?? "Неизвестно"}",
            $"Количество: {quantity} шт.",
            $"Номер заказа: {order.Id}",
            $"Дата: {order.OrderDate:dd.MM.yyyy HH:mm}",
            $"Сумма: {order.TotalAmount:F2} руб."
        };
        OutputChanged?.Invoke(this, lines);
        
        return order;
    }
    
    public List<Order> GetUserOrders(bool ascending = true)
    {
        if (!IsLoggedIn)
            return new List<Order>();
        
        return _orderService.GetUserOrders(_currentUser!.Id, ascending);
    }
    
    public List<OrderItem> GetOrderItems(int orderId)
    {
        return _orderService.GetOrderItems(orderId);
    }
}

