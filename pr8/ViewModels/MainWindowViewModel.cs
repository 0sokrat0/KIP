using System.Collections.ObjectModel;
using System.Windows.Input;
using pr8.Infrastructure;
using pr8.Models;
using pr8.Services;

namespace pr8.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMarketplaceFacade _facade;
    
    private ObservableCollection<string> _outputLines = new();
    private ObservableCollection<Product> _products = new();
    private ObservableCollection<CartItemViewModel> _cartItems = new();
    private ObservableCollection<OrderViewModel> _orders = new();
    private ObservableCollection<PickupPoint> _pickupPoints = new();
    
    private string _username = "";
    private string _password = "";
    private string _confirmPassword = "";
    private string _loginUsername = "";
    private string _loginPassword = "";
    private string _selectedProductId = "";
    private string _quantity = "1";
    private PickupPoint? _selectedPickupPoint;
    private bool _ordersAscending = true;
    private string _statusMessage = "";
    private bool _isStatusSuccess = false;
    
    public ObservableCollection<string> OutputLines
    {
        get => _outputLines;
        set => SetProperty(ref _outputLines, value);
    }
    
    public ObservableCollection<Product> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }
    
    public ObservableCollection<CartItemViewModel> CartItems
    {
        get => _cartItems;
        set => SetProperty(ref _cartItems, value);
    }
    
    public ObservableCollection<OrderViewModel> Orders
    {
        get => _orders;
        set => SetProperty(ref _orders, value);
    }
    
    public ObservableCollection<PickupPoint> PickupPoints
    {
        get => _pickupPoints;
        set => SetProperty(ref _pickupPoints, value);
    }
    
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }
    
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }
    
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }
    
    public string LoginUsername
    {
        get => _loginUsername;
        set => SetProperty(ref _loginUsername, value);
    }
    
    public string LoginPassword
    {
        get => _loginPassword;
        set => SetProperty(ref _loginPassword, value);
    }
    
    public string SelectedProductId
    {
        get => _selectedProductId;
        set => SetProperty(ref _selectedProductId, value);
    }
    
    public string Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }
    
    public PickupPoint? SelectedPickupPoint
    {
        get => _selectedPickupPoint;
        set
        {
            if (SetProperty(ref _selectedPickupPoint, value))
            {
                RaisePropertyChanged(nameof(SelectedPickupPointId));
            }
        }
    }
    
    public int SelectedPickupPointId => _selectedPickupPoint?.Id ?? 0;
    
    public bool OrdersAscending
    {
        get => _ordersAscending;
        set
        {
            if (SetProperty(ref _ordersAscending, value))
            {
                LoadOrders();
            }
        }
    }
    
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (SetProperty(ref _statusMessage, value))
            {
                RaisePropertyChanged(nameof(StatusBackgroundColor));
            }
        }
    }
    
    public bool IsStatusSuccess
    {
        get => _isStatusSuccess;
        set
        {
            if (SetProperty(ref _isStatusSuccess, value))
            {
                RaisePropertyChanged(nameof(StatusBackgroundColor));
            }
        }
    }
    
    public string StatusBackgroundColor => _isStatusSuccess ? "#4CAF50" : "#F44336";
    
    public bool IsLoggedIn => _facade.IsLoggedIn;
    public string CurrentUsername => _facade.CurrentUser?.Username ?? "";
    public decimal CartTotal => _facade.GetCartTotal();
    
    public ICommand RegisterCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand LoadProductsCommand { get; }
    public ICommand AddToCartCommand { get; }
    public ICommand BuySingleItemCommand { get; }
    public ICommand LoadCartCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand BuyCartCommand { get; }
    public ICommand LoadOrdersCommand { get; }
    public ICommand ExitCommand { get; }
    
    public MainWindowViewModel()
    {
        _facade = ServiceContainer.GetMarketplaceFacade();
        _facade.OutputChanged += OnOutputChanged;
        _facade.UserChanged += OnUserChanged;
        
        RegisterCommand = new RelayCommand(Register);
        LoginCommand = new RelayCommand(Login);
        LogoutCommand = new RelayCommand(Logout, () => IsLoggedIn);
        LoadProductsCommand = new RelayCommand(LoadProducts);
        AddToCartCommand = new RelayCommand(AddToCart, () => IsLoggedIn);
        BuySingleItemCommand = new RelayCommand(BuySingleItem, () => IsLoggedIn);
        LoadCartCommand = new RelayCommand(LoadCart, () => IsLoggedIn);
        RemoveFromCartCommand = new RelayCommand<int>(RemoveFromCart, (p) => IsLoggedIn);
        BuyCartCommand = new RelayCommand(BuyCart, () => IsLoggedIn);
        LoadOrdersCommand = new RelayCommand(LoadOrders, () => IsLoggedIn);
        ExitCommand = new RelayCommand(Exit);
        
        LoadProducts();
        LoadPickupPoints();
        ShowWelcomeMessage();
    }
    
    private void LoadPickupPoints()
    {
        var points = _facade.GetAllPickupPoints();
        PickupPoints.Clear();
        foreach (var point in points)
        {
            PickupPoints.Add(point);
        }
    }
    
    private void ShowWelcomeMessage()
    {
        var lines = new[]
        {
            "Добро пожаловать в маркетплейс GMWOG||GG.MOW||WONGG!",
            "",
            "Вы можете просматривать товары без регистрации.",
            "Для добавления товаров в корзину и оформления заказов необходимо войти в систему."
        };
        SetOutputLines(lines);
    }
    
    private void OnOutputChanged(object? sender, string[] lines)
    {
        SetOutputLines(lines);
    }
    
    private void OnUserChanged(object? sender, EventArgs e)
    {
        RaisePropertyChanged(nameof(IsLoggedIn));
        RaisePropertyChanged(nameof(CurrentUsername));
        CommandManager.InvalidateRequerySuggested();
        LoadCart();
    }
    
    private void SetOutputLines(string[] lines)
    {
        OutputLines.Clear();
        foreach (var line in lines)
        {
            OutputLines.Add(line);
        }
    }
    
    private void Register()
    {
        try
        {
            var success = _facade.Register(Username, Password, ConfirmPassword);
            if (success)
            {
                StatusMessage = "Регистрация успешна! Теперь вы можете войти в систему.";
                IsStatusSuccess = true;
                Username = "";
                Password = "";
                ConfirmPassword = "";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка регистрации: {ex.Message}";
            IsStatusSuccess = false;
        }
    }
    
    private void Login()
    {
        try
        {
            var user = _facade.Login(LoginUsername, LoginPassword);
            if (user != null)
            {
                StatusMessage = $"Добро пожаловать, {user.Username}!";
                IsStatusSuccess = true;
                LoginUsername = "";
                LoginPassword = "";
                LoadCart();
            }
            else
            {
                StatusMessage = "Неверное имя пользователя или пароль.";
                IsStatusSuccess = false;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка входа: {ex.Message}";
            IsStatusSuccess = false;
        }
    }
    
    private void Logout()
    {
        _facade.Logout();
        CartItems.Clear();
        Orders.Clear();
    }
    
    private void LoadProducts()
    {
        var products = _facade.GetAllProducts();
        Products.Clear();
        foreach (var product in products)
        {
            Products.Add(product);
        }
    }
    
    private void AddToCart()
    {
        if (!int.TryParse(SelectedProductId, out int productId) || productId <= 0)
        {
            SetOutputLines(new[] { "Ошибка: Введите корректный ID товара" });
            return;
        }
        
        if (!int.TryParse(Quantity, out int quantity) || quantity <= 0)
        {
            SetOutputLines(new[] { "Ошибка: Введите корректное количество" });
            return;
        }
        
        _facade.AddToCart(productId, quantity);
        LoadCart();
        SelectedProductId = "";
        Quantity = "1";
    }
    
    private void BuySingleItem()
    {
        if (!int.TryParse(SelectedProductId, out int productId) || productId <= 0)
        {
            StatusMessage = "Ошибка: Введите корректный ID товара";
            IsStatusSuccess = false;
            return;
        }
        
        if (!int.TryParse(Quantity, out int quantity) || quantity <= 0)
        {
            StatusMessage = "Ошибка: Введите корректное количество";
            IsStatusSuccess = false;
            return;
        }
        
        if (SelectedPickupPointId <= 0)
        {
            StatusMessage = "Ошибка: Выберите пункт выдачи заказов";
            IsStatusSuccess = false;
            return;
        }
        
        try
        {
            var order = _facade.CreateSingleItemOrder(productId, quantity, SelectedPickupPointId);
            StatusMessage = $"Заказ №{order.Id} успешно оформлен! Сумма: {order.TotalAmount:F2} руб.";
            IsStatusSuccess = true;
            LoadProducts();
            LoadCart();
            LoadOrders();
            SelectedProductId = "";
            Quantity = "1";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            IsStatusSuccess = false;
        }
    }
    
    private void LoadCart()
    {
        if (!IsLoggedIn)
            return;
        
        var items = _facade.GetCartItems();
        CartItems.Clear();
        
        foreach (var item in items)
        {
            var product = _facade.GetProductById(item.ProductId);
            if (product != null)
            {
                CartItems.Add(new CartItemViewModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    Total = product.Price * item.Quantity
                });
            }
        }
        
        RaisePropertyChanged(nameof(CartTotal));
    }
    
    private void RemoveFromCart(int productId)
    {
        _facade.RemoveFromCart(productId);
        LoadCart();
    }
    
    public void RemoveFromCartByProductId(int productId)
    {
        RemoveFromCart(productId);
    }
    
    private void BuyCart()
    {
        if (SelectedPickupPointId <= 0)
        {
            StatusMessage = "Ошибка: Выберите пункт выдачи заказов";
            IsStatusSuccess = false;
            return;
        }
        
        try
        {
            var order = _facade.CreateOrder(SelectedPickupPointId);
            StatusMessage = $"Заказ №{order.Id} успешно оформлен! Сумма: {order.TotalAmount:F2} руб.";
            IsStatusSuccess = true;
            LoadProducts();
            LoadCart();
            LoadOrders();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
            IsStatusSuccess = false;
        }
    }
    
    private void LoadOrders()
    {
        if (!IsLoggedIn)
            return;
        
        var orders = _facade.GetUserOrders(OrdersAscending);
        Orders.Clear();
        
        foreach (var order in orders)
        {
            var pickupPoint = _facade.GetAllPickupPoints().FirstOrDefault(p => p.Id == order.PickupPointId);
            var orderItems = _facade.GetOrderItems(order.Id);
            
            Orders.Add(new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                PickupPointName = pickupPoint?.Name ?? "Неизвестно",
                TotalAmount = order.TotalAmount,
                Items = orderItems.Select(oi =>
                {
                    var product = _facade.GetProductById(oi.ProductId);
                    return $"{product?.Name ?? "Неизвестно"} x{oi.Quantity} - {oi.Price * oi.Quantity:F2} руб.";
                }).ToList()
            });
        }
    }
    
    private void Exit()
    {
        Environment.Exit(0);
    }
}

public class CartItemViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
}

public class OrderViewModel
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string PickupPointName { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public List<string> Items { get; set; } = new();
}

