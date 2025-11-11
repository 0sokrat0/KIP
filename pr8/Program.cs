using pr8.Data;
using pr8.Models;
using pr8.Repositories;
using pr8.Services;

namespace pr8;

class Program
{
    private static DatabaseContext? _dbContext;
    private static AuthService? _authService;
    private static ProductService? _productService;
    private static CartService? _cartService;
    private static OrderService? _orderService;
    private static PickupPointService? _pickupPointService;
    private static User? _currentUser;
    
    static void Main(string[] args)
    {
        try
        {
            InitializeApplication();
            
            Console.WriteLine("МАРКЕТПЛЕЙС GMWOG||GG.MOW||WONGG");
            Console.WriteLine("Добро пожаловать!");
            Console.WriteLine();
            
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    if (_currentUser == null)
                    {
                        ShowMainMenu();
                        string? choice = Console.ReadLine();
                        
                        switch (choice)
                        {
                            case "1":
                                Register();
                                break;
                            case "2":
                                Login();
                                break;
                            case "3":
                                ViewProducts();
                                break;
                            case "0":
                                isRunning = false;
                                Console.WriteLine("До свидания!");
                                break;
                            default:
                                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                                break;
                        }
                    }
                    else
                    {
                        ShowUserMenu();
                        string? choice = Console.ReadLine();
                        
                        switch (choice)
                        {
                            case "1":
                                ViewProducts();
                                break;
                            case "2":
                                ViewCart();
                                break;
                            case "3":
                                AddToCart();
                                break;
                            case "4":
                                RemoveFromCart();
                                break;
                            case "5":
                                BuyFromCart();
                                break;
                            case "6":
                                BuySingleItem();
                                break;
                            case "7":
                                ViewOrderHistory();
                                break;
                            case "8":
                                Logout();
                                break;
                            case "0":
                                isRunning = false;
                                Console.WriteLine("До свидания!");
                                break;
                            default:
                                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
                
                if (isRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    static void InitializeApplication()
    {
        string dbPath = "Marketplace.db";
        string connectionString = $"Data Source={dbPath};";
        
        _dbContext = new DatabaseContext(connectionString);
        _dbContext.InitializeDatabase();
        
        var userRepository = new UserRepository(_dbContext);
        var productRepository = new ProductRepository(_dbContext);
        var pickupPointRepository = new PickupPointRepository(_dbContext);
        var cartRepository = new CartRepository(_dbContext);
        var cartItemRepository = new CartItemRepository(_dbContext);
        var orderRepository = new OrderRepository(_dbContext);
        var orderItemRepository = new OrderItemRepository(_dbContext);
        
        _authService = new AuthService(userRepository);
        _productService = new ProductService(productRepository);
        _pickupPointService = new PickupPointService(pickupPointRepository);
        _cartService = new CartService(cartRepository, cartItemRepository, productRepository);
        _orderService = new OrderService(orderRepository, orderItemRepository, productRepository, _cartService);
    }
    
    static void ShowMainMenu()
    {
        Console.WriteLine("ГЛАВНОЕ МЕНЮ");
        Console.WriteLine("1. Регистрация");
        Console.WriteLine("2. Вход в аккаунт");
        Console.WriteLine("3. Просмотр товаров");
        Console.WriteLine("0. Выход");
        Console.Write("Ваш выбор: ");
    }
    
    static void ShowUserMenu()
    {
        Console.WriteLine($"МЕНЮ ПОЛЬЗОВАТЕЛЯ ({_currentUser?.Username})");
        Console.WriteLine("1. Просмотр товаров");
        Console.WriteLine("2. Просмотр корзины");
        Console.WriteLine("3. Добавить товар в корзину");
        Console.WriteLine("4. Удалить товар из корзины");
        Console.WriteLine("5. Оформить заказ из корзины");
        Console.WriteLine("6. Купить товар сразу");
        Console.WriteLine("7. История заказов");
        Console.WriteLine("8. Выйти из аккаунта");
        Console.WriteLine("0. Выход из программы");
        Console.Write("Ваш выбор: ");
    }
    
    static void Register()
    {
        Console.Clear();
        Console.WriteLine("РЕГИСТРАЦИЯ");
        Console.Write("Введите имя пользователя: ");
        string? username = Console.ReadLine();
        
        Console.Write("Введите пароль: ");
        string? password = Console.ReadLine();
        
        Console.Write("Подтвердите пароль: ");
        string? confirmPassword = Console.ReadLine();
        
        if (_authService != null && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(confirmPassword))
        {
            _authService.Register(username, password, confirmPassword);
        }
    }
    
    static void Login()
    {
        Console.Clear();
        Console.WriteLine("ВХОД В АККАУНТ");
        Console.Write("Введите имя пользователя: ");
        string? username = Console.ReadLine();
        
        Console.Write("Введите пароль: ");
        string? password = Console.ReadLine();
        
        if (_authService != null && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            _currentUser = _authService.Login(username, password);
        }
    }
    
    static void Logout()
    {
        _currentUser = null;
        Console.WriteLine("Вы вышли из аккаунта");
    }
    
    static void ViewProducts()
    {
        Console.Clear();
        if (_productService != null)
        {
            var products = _productService.GetAllProducts();
            _productService.DisplayProducts(products);
        }
    }
    
    static void ViewCart()
    {
        Console.Clear();
        if (_cartService != null && _currentUser != null)
        {
            _cartService.DisplayCart(_currentUser.Id);
        }
    }
    
    static void AddToCart()
    {
        Console.Clear();
        if (_productService == null || _cartService == null || _currentUser == null)
        {
            return;
        }
        
        ViewProducts();
        Console.WriteLine();
        Console.Write("Введите ID товара: ");
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            Console.Write("Введите количество: ");
            if (int.TryParse(Console.ReadLine(), out int quantity))
            {
                _cartService.AddToCart(_currentUser.Id, productId, quantity);
            }
            else
            {
                Console.WriteLine("Неверное количество");
            }
        }
        else
        {
            Console.WriteLine("Неверный ID товара");
        }
    }
    
    static void RemoveFromCart()
    {
        Console.Clear();
        if (_cartService == null || _currentUser == null)
        {
            return;
        }
        
        ViewCart();
        Console.WriteLine();
        Console.Write("Введите ID товара для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            _cartService.RemoveFromCart(_currentUser.Id, productId);
        }
        else
        {
            Console.WriteLine("Неверный ID товара");
        }
    }
    
    static void BuyFromCart()
    {
        Console.Clear();
        if (_cartService == null || _orderService == null || _pickupPointService == null || _currentUser == null)
        {
            return;
        }
        
        var cartItems = _cartService.GetCartItems(_currentUser.Id);
        if (cartItems.Count == 0)
        {
            Console.WriteLine("Корзина пуста");
            return;
        }
        
        ViewCart();
        Console.WriteLine();
        
        var pickupPoints = _pickupPointService.GetAllPickupPoints();
        _pickupPointService.DisplayPickupPoints(pickupPoints);
        Console.WriteLine();
        
        Console.Write("Выберите ПВЗ (ID): ");
        if (int.TryParse(Console.ReadLine(), out int pickupPointId))
        {
            var pickupPoint = _pickupPointService.GetPickupPointById(pickupPointId);
            if (pickupPoint == null)
            {
                Console.WriteLine("ПВЗ не найдено");
                return;
            }
            
            try
            {
                var order = _orderService.CreateOrder(_currentUser.Id, pickupPointId, cartItems);
                Console.WriteLine($"Заказ #{order.Id} успешно оформлен!");
                Console.WriteLine($"Сумма: {order.TotalAmount:F2} руб.");
                Console.WriteLine($"ПВЗ: {pickupPoint.Name}, {pickupPoint.Address}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при оформлении заказа: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Неверный ID ПВЗ");
        }
    }
    
    static void BuySingleItem()
    {
        Console.Clear();
        if (_productService == null || _orderService == null || _pickupPointService == null || _currentUser == null)
        {
            return;
        }
        
        ViewProducts();
        Console.WriteLine();
        Console.Write("Введите ID товара: ");
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                Console.WriteLine("Товар не найден");
                return;
            }
            
            Console.Write("Введите количество: ");
            if (int.TryParse(Console.ReadLine(), out int quantity))
            {
                if (quantity <= 0)
                {
                    Console.WriteLine("Количество должно быть больше нуля");
                    return;
                }
                
                var pickupPoints = _pickupPointService.GetAllPickupPoints();
                _pickupPointService.DisplayPickupPoints(pickupPoints);
                Console.WriteLine();
                
                Console.Write("Выберите ПВЗ (ID): ");
                if (int.TryParse(Console.ReadLine(), out int pickupPointId))
                {
                    var pickupPoint = _pickupPointService.GetPickupPointById(pickupPointId);
                    if (pickupPoint == null)
                    {
                        Console.WriteLine("ПВЗ не найдено");
                        return;
                    }
                    
                    try
                    {
                        var order = _orderService.CreateSingleItemOrder(_currentUser.Id, productId, quantity, pickupPointId);
                        Console.WriteLine($"Заказ #{order.Id} успешно оформлен!");
                        Console.WriteLine($"Товар: {product.Name}");
                        Console.WriteLine($"Количество: {quantity}");
                        Console.WriteLine($"Сумма: {order.TotalAmount:F2} руб.");
                        Console.WriteLine($"ПВЗ: {pickupPoint.Name}, {pickupPoint.Address}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при оформлении заказа: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный ID ПВЗ");
                }
            }
            else
            {
                Console.WriteLine("Неверное количество");
            }
        }
        else
        {
            Console.WriteLine("Неверный ID товара");
        }
    }
    
    static void ViewOrderHistory()
    {
        Console.Clear();
        if (_orderService == null || _currentUser == null)
        {
            return;
        }
        
        Console.WriteLine("Сортировка:");
        Console.WriteLine("1. От старых к новым");
        Console.WriteLine("2. От новых к старым");
        Console.Write("Ваш выбор: ");
        
        bool sortAscending = true;
        if (Console.ReadLine() == "2")
        {
            sortAscending = false;
        }
        
        var orders = _orderService.GetUserOrders(_currentUser.Id, sortAscending);
        _orderService.DisplayOrders(orders);
    }
}
