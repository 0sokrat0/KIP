using System;

namespace StoreManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система учёта товаров в магазине ===");
            Console.WriteLine("Добро пожаловать!");
            
            Store store = new Store();
            store.InitializeWithTestData();
            
            bool isRunning = true;
            while (isRunning)
            {
                try
                {
                    ShowMainMenu();
                    string choice = Console.ReadLine();
                    
                    switch (choice)
                    {
                        case "1":
                            store.AddProduct();
                            break;
                        case "2":
                            store.RemoveProduct();
                            break;
                        case "3":
                            store.OrderSupply();
                            break;
                        case "4":
                            store.SellProduct();
                            break;
                        case "5":
                            store.SearchProducts();
                            break;
                        case "6":
                            store.ShowAllProducts();
                            break;
                        case "7":
                            store.GenerateSalesReport();
                            break;
                        case "8":
                            store.UndoLastSale();
                            break;
                        case "9":
                            store.ShowSalesHistory();
                            break;
                        case "0":
                            isRunning = false;
                            Console.WriteLine("Спасибо за использование системы! До свидания!");
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                    
                    if (isRunning)
                    {
                        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        Console.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }
        
        static void ShowMainMenu()
        {
            Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Удалить товар");
            Console.WriteLine("3. Заказать поставку товара");
            Console.WriteLine("4. Продать товар");
            Console.WriteLine("5. Поиск товаров");
            Console.WriteLine("6. Показать все товары");
            Console.WriteLine("7. Отчёт о продажах");
            Console.WriteLine("8. Отменить последнюю продажу");
            Console.WriteLine("9. История продаж");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
        }
    }
}
