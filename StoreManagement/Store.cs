using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManagement
{
    public class Store
    {
        private List<Product> products;
        private List<Sale> sales;
        private Stack<Sale> salesHistory;
        private int nextProductId;
        
        public Store()
        {
            products = new List<Product>();
            sales = new List<Sale>();
            salesHistory = new Stack<Sale>();
            nextProductId = 1;
        }
        
        public void InitializeWithTestData()
        {
            try
            {
                AddProductInternal("Смартфон Samsung Galaxy", 45000, 10, Category.Electronics);
                AddProductInternal("Джинсы Levis", 3500, 25, Category.Clothing);
                AddProductInternal("Молоко 1л", 80, 50, Category.Food);
                AddProductInternal("Книга 'Война и мир'", 1200, 15, Category.Books);
                AddProductInternal("Футбольный мяч", 2500, 8, Category.Sports);
                
                Console.WriteLine("Тестовые данные успешно загружены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке тестовых данных: {ex.Message}");
            }
        }
        
        public void AddProduct()
        {
            try
            {
                Console.WriteLine("\n=== ДОБАВЛЕНИЕ ТОВАРА ===");
                
                Console.Write("Введите название товара: ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ошибка: Название товара не может быть пустым.");
                    return;
                }
                
                Console.Write("Введите цену товара: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                {
                    Console.WriteLine("Ошибка: Введите корректную цену (неотрицательное число).");
                    return;
                }
                
                Console.Write("Введите количество товара: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
                {
                    Console.WriteLine("Ошибка: Введите корректное количество (неотрицательное число).");
                    return;
                }
                
                CategoryHelper.ShowAllCategories();
                Console.Write("Выберите категорию (номер): ");
                if (!int.TryParse(Console.ReadLine(), out int categoryNumber))
                {
                    Console.WriteLine("Ошибка: Введите корректный номер категории.");
                    return;
                }
                
                Category? category = CategoryHelper.GetCategoryByNumber(categoryNumber);
                if (category == null)
                {
                    Console.WriteLine("Ошибка: Неверный номер категории.");
                    return;
                }
                
                AddProductInternal(name, price, quantity, category.Value);
                Console.WriteLine("Товар успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении товара: {ex.Message}");
            }
        }
        
        private void AddProductInternal(string name, decimal price, int quantity, Category category)
        {
            Product product = new Product(name, price, quantity, category);
            products.Add(product);
        }
        
        public void RemoveProduct()
        {
            try
            {
                Console.WriteLine("\n=== УДАЛЕНИЕ ТОВАРА ===");
                
                if (products.Count == 0)
                {
                    Console.WriteLine("Список товаров пуст.");
                    return;
                }
                
                Console.Write("Введите код товара для удаления: ");
                string code = Console.ReadLine();
                
                Product product = products.FirstOrDefault(p => p.Code == code);
                if (product == null)
                {
                    Console.WriteLine("Товар с указанным кодом не найден.");
                    return;
                }
                
                Console.WriteLine($"Найден товар: {product.GetShortInfo()}");
                Console.Write("Вы уверены, что хотите удалить этот товар? (да/нет): ");
                string confirmation = Console.ReadLine()?.ToLower();
                
                if (confirmation == "да" || confirmation == "yes" || confirmation == "y")
                {
                    products.Remove(product);
                    Console.WriteLine("Товар успешно удален!");
                }
                else
                {
                    Console.WriteLine("Удаление отменено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении товара: {ex.Message}");
            }
        }
        
        public void OrderSupply()
        {
            try
            {
                Console.WriteLine("\n=== ЗАКАЗ ПОСТАВКИ ТОВАРА ===");
                
                if (products.Count == 0)
                {
                    Console.WriteLine("Список товаров пуст.");
                    return;
                }
                
                Console.Write("Введите код товара для заказа поставки: ");
                string code = Console.ReadLine();
                
                Product product = products.FirstOrDefault(p => p.Code == code);
                if (product == null)
                {
                    Console.WriteLine("Товар с указанным кодом не найден.");
                    return;
                }
                
                Console.WriteLine($"Товар: {product.GetShortInfo()}");
                Console.Write("Введите количество для заказа: ");
                
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Ошибка: Введите корректное количество (положительное число).");
                    return;
                }
                
                product.AddQuantity(quantity);
                Console.WriteLine($"Поставка успешно заказана! Добавлено {quantity} единиц товара.");
                Console.WriteLine($"Новое количество: {product.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при заказе поставки: {ex.Message}");
            }
        }
        
        public void SellProduct()
        {
            try
            {
                Console.WriteLine("\n=== ПРОДАЖА ТОВАРА ===");
                
                if (products.Count == 0)
                {
                    Console.WriteLine("Список товаров пуст.");
                    return;
                }
                
                Console.Write("Введите код товара для продажи: ");
                string code = Console.ReadLine();
                
                Product product = products.FirstOrDefault(p => p.Code == code);
                if (product == null)
                {
                    Console.WriteLine("Товар с указанным кодом не найден.");
                    return;
                }
                
                if (!product.IsInStock)
                {
                    Console.WriteLine("Товар отсутствует на складе.");
                    return;
                }
                
                Console.WriteLine($"Товар: {product.GetShortInfo()}");
                Console.Write("Введите количество для продажи: ");
                
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Ошибка: Введите корректное количество (положительное число).");
                    return;
                }
                
                if (product.TrySell(quantity))
                {
                    decimal totalPrice = product.Price * quantity;
                    
                    Sale sale = new Sale(product.Code, product.Name, product.Price, quantity, product.Category);
                    sales.Add(sale);
                    salesHistory.Push(sale);
                    
                    Console.WriteLine($"Продажа успешно завершена!");
                    Console.WriteLine($"Продано: {quantity} единиц");
                    Console.WriteLine($"Цена за единицу: {product.Price:C}");
                    Console.WriteLine($"Общая сумма: {totalPrice:C}");
                    Console.WriteLine($"Остаток на складе: {product.Quantity}");
                }
                else
                {
                    Console.WriteLine($"Недостаточно товара на складе. Доступно: {product.Quantity}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при продаже товара: {ex.Message}");
            }
        }
        
        public void SearchProducts()
        {
            try
            {
                Console.WriteLine("\n=== ПОИСК ТОВАРОВ ===");
                Console.WriteLine("1. Поиск по коду");
                Console.WriteLine("2. Поиск по названию");
                Console.WriteLine("3. Поиск по категории");
                Console.Write("Выберите тип поиска: ");
                
                string choice = Console.ReadLine();
                List<Product> searchResults = new List<Product>();
                
                switch (choice)
                {
                    case "1":
                        Console.Write("Введите код товара: ");
                        string code = Console.ReadLine();
                        searchResults = products.Where(p => p.Code.Contains(code, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                        
                    case "2":
                        Console.Write("Введите название товара: ");
                        string name = Console.ReadLine();
                        searchResults = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                        
                    case "3":
                        CategoryHelper.ShowAllCategories();
                        Console.Write("Выберите категорию (номер): ");
                        if (int.TryParse(Console.ReadLine(), out int categoryNumber))
                        {
                            Category? category = CategoryHelper.GetCategoryByNumber(categoryNumber);
                            if (category != null)
                            {
                                searchResults = products.Where(p => p.Category == category.Value).ToList();
                            }
                        }
                        break;
                        
                    default:
                        Console.WriteLine("Неверный выбор.");
                        return;
                }
                
                if (searchResults.Count == 0)
                {
                    Console.WriteLine("Товары не найдены.");
                }
                else
                {
                    Console.WriteLine($"\nНайдено товаров: {searchResults.Count}");
                    Console.WriteLine(new string('=', 50));
                    foreach (var product in searchResults)
                    {
                        Console.WriteLine(product.ToString());
                        Console.WriteLine(new string('-', 30));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске товаров: {ex.Message}");
            }
        }
        
        public void ShowAllProducts()
        {
            try
            {
                Console.WriteLine("\n=== ВСЕ ТОВАРЫ ===");
                
                if (products.Count == 0)
                {
                    Console.WriteLine("Список товаров пуст.");
                    return;
                }
                
                Console.WriteLine($"Всего товаров: {products.Count}");
                Console.WriteLine(new string('=', 50));
                
                foreach (var product in products)
                {
                    Console.WriteLine(product.ToString());
                    Console.WriteLine(new string('-', 30));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отображении товаров: {ex.Message}");
            }
        }
        
        public void GenerateSalesReport()
        {
            try
            {
                Console.WriteLine("\n=== ОТЧЁТ О ПРОДАЖАХ ===");
                
                if (sales.Count == 0)
                {
                    Console.WriteLine("Продажи не найдены.");
                    return;
                }
                
                Console.WriteLine($"Всего продаж: {sales.Count}");
                Console.WriteLine(new string('=', 80));
                
                decimal totalRevenue = 0;
                int totalItemsSold = 0;
                
                foreach (var sale in sales.OrderBy(s => s.SaleDate))
                {
                    Console.WriteLine(sale.ToString());
                    Console.WriteLine(new string('-', 50));
                    totalRevenue += sale.TotalAmount;
                    totalItemsSold += sale.QuantitySold;
                }
                
                Console.WriteLine(new string('=', 80));
                Console.WriteLine($"ИТОГО:");
                Console.WriteLine($"Общее количество проданных товаров: {totalItemsSold} шт.");
                Console.WriteLine($"Общая сумма продаж: {totalRevenue:C}");
                Console.WriteLine($"Средняя сумма за продажу: {(totalRevenue / sales.Count):C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации отчёта о продажах: {ex.Message}");
            }
        }
        
        public void UndoLastSale()
        {
            try
            {
                Console.WriteLine("\n=== ОТМЕНА ПОСЛЕДНЕЙ ПРОДАЖИ ===");
                
                if (salesHistory.Count == 0)
                {
                    Console.WriteLine("История продаж пуста. Нет продаж для отмены.");
                    return;
                }
                
                Sale lastSale = salesHistory.Pop();
                
                Product product = products.FirstOrDefault(p => p.Code == lastSale.ProductCode);
                if (product == null)
                {
                    Console.WriteLine("Ошибка: Товар для отмены продажи не найден.");
                    salesHistory.Push(lastSale);
                    return;
                }
                
                product.AddQuantity(lastSale.QuantitySold);
                sales.Remove(lastSale);
                
                Console.WriteLine("Последняя продажа успешно отменена!");
                Console.WriteLine($"Отменённая продажа: {lastSale.GetShortInfo()}");
                Console.WriteLine($"Товар возвращён на склад. Новое количество: {product.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отмене последней продажи: {ex.Message}");
            }
        }
        
        public void ShowSalesHistory()
        {
            try
            {
                Console.WriteLine("\n=== ИСТОРИЯ ПРОДАЖ ===");
                
                if (salesHistory.Count == 0)
                {
                    Console.WriteLine("История продаж пуста.");
                    return;
                }
                
                Console.WriteLine($"Всего операций в истории: {salesHistory.Count}");
                Console.WriteLine(new string('=', 80));
                
                var historyArray = salesHistory.ToArray();
                for (int i = historyArray.Length - 1; i >= 0; i--)
                {
                    Console.WriteLine($"Операция #{historyArray.Length - i}:");
                    Console.WriteLine(historyArray[i].ToString());
                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отображении истории продаж: {ex.Message}");
            }
        }
    }
}
