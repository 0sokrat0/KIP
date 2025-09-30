using System;
using System.Collections.Generic;
using System.Linq;
using LibraryApp.Models;
using LibraryApp.Services;

namespace LibraryApp
{
    class Program
    {
        private static LibraryManager _libraryManager;

        static void Main(string[] args)
        {
            _libraryManager = new LibraryManager();
            
            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();
                
                try
                {
                    HandleMainMenuChoice(choice);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
                
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("=== СИСТЕМА УЧЁТА КНИГ В БИБЛИОТЕКЕ ===");
            Console.WriteLine("1. Показать все книги");
            Console.WriteLine("2. Добавить книгу");
            Console.WriteLine("3. Удалить книгу");
            Console.WriteLine("4. Поиск книг");
            Console.WriteLine("5. Сортировка книг");
            Console.WriteLine("6. Самая дорогая/дешёвая книга");
            Console.WriteLine("7. Группировка по авторам");
            Console.WriteLine("8. Вставить блок книг");
            Console.WriteLine("9. Корзина");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
        }

        static void HandleMainMenuChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    _libraryManager.DisplayAllBooks();
                    break;
                case "2":
                    AddBook();
                    break;
                case "3":
                    RemoveBook();
                    break;
                case "4":
                    ShowSearchMenu();
                    break;
                case "5":
                    ShowSortMenu();
                    break;
                case "6":
                    ShowPriceRange();
                    break;
                case "7":
                    ShowBooksByAuthor();
                    break;
                case "8":
                    ImportBooks();
                    break;
                case "9":
                    ShowShoppingCartMenu();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        static void AddBook()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ КНИГИ ===");
            
            Console.Write("Введите название: ");
            var title = Console.ReadLine();
            
            Console.Write("Введите автора: ");
            var author = Console.ReadLine();
            
            Console.WriteLine("Доступные жанры:");
            var genres = Enum.GetValues<Genre>();
            for (int i = 0; i < genres.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {genres[i]}");
            }
            
            Console.Write("Выберите жанр (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int genreIndex) || genreIndex < 1 || genreIndex > genres.Length)
            {
                Console.WriteLine("Неверный выбор жанра!");
                return;
            }
            
            Console.Write("Введите год издания: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Неверный формат года!");
                return;
            }
            
            Console.Write("Введите цену: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Неверный формат цены!");
                return;
            }
            
            _libraryManager.AddBook(title, author, genres[genreIndex - 1], year, price);
        }

        static void RemoveBook()
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ КНИГИ ===");
            _libraryManager.DisplayAllBooks();
            
            Console.Write("Введите ID книги для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _libraryManager.RemoveBook(id);
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void ShowSearchMenu()
        {
            Console.WriteLine("\n=== ПОИСК КНИГ ===");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По автору");
            Console.WriteLine("3. По жанру");
            Console.Write("Выберите тип поиска: ");
            
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Введите название для поиска: ");
                    var title = Console.ReadLine();
                    var booksByTitle = _libraryManager.SearchByTitle(title);
                    Console.WriteLine($"\nНайдено книг: {booksByTitle.Count}");
                    _libraryManager.DisplayBooks(booksByTitle);
                    break;
                case "2":
                    Console.Write("Введите автора для поиска: ");
                    var author = Console.ReadLine();
                    var booksByAuthor = _libraryManager.SearchByAuthor(author);
                    Console.WriteLine($"\nНайдено книг: {booksByAuthor.Count}");
                    _libraryManager.DisplayBooks(booksByAuthor);
                    break;
                case "3":
                    Console.WriteLine("Доступные жанры:");
                    var genres = Enum.GetValues<Genre>();
                    for (int i = 0; i < genres.Length; i++)
                    {
                        Console.WriteLine($"{i + 1}. {genres[i]}");
                    }
                    Console.Write("Выберите жанр (номер): ");
                    if (int.TryParse(Console.ReadLine(), out int genreIndex) && genreIndex >= 1 && genreIndex <= genres.Length)
                    {
                        var booksByGenre = _libraryManager.SearchByGenre(genres[genreIndex - 1]);
                        Console.WriteLine($"\nНайдено книг: {booksByGenre.Count}");
                        _libraryManager.DisplayBooks(booksByGenre);
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор жанра!");
                    }
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        static void ShowSortMenu()
        {
            Console.WriteLine("\n=== СОРТИРОВКА КНИГ ===");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По году издания");
            Console.Write("Выберите тип сортировки: ");
            
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var booksByTitle = _libraryManager.SortByTitle();
                    Console.WriteLine("\nКниги, отсортированные по названию:");
                    _libraryManager.DisplayBooks(booksByTitle);
                    break;
                case "2":
                    var booksByYear = _libraryManager.SortByYear();
                    Console.WriteLine("\nКниги, отсортированные по году издания:");
                    _libraryManager.DisplayBooks(booksByYear);
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        static void ShowPriceRange()
        {
            Console.WriteLine("\n=== САМАЯ ДОРОГАЯ И ДЕШЁВАЯ КНИГИ ===");
            
            var mostExpensive = _libraryManager.GetMostExpensiveBook();
            var cheapest = _libraryManager.GetCheapestBook();
            
            if (mostExpensive != null)
            {
                Console.WriteLine($"Самая дорогая книга: {mostExpensive}");
            }
            
            if (cheapest != null)
            {
                Console.WriteLine($"Самая дешёвая книга: {cheapest}");
            }
        }

        static void ShowBooksByAuthor()
        {
            Console.WriteLine("\n=== ГРУППИРОВКА ПО АВТОРАМ ===");
            var booksByAuthor = _libraryManager.GetBooksByAuthor();
            
            foreach (var author in booksByAuthor)
            {
                Console.WriteLine($"{author.Key}: {author.Value} книг(и)");
            }
        }

        static void ImportBooks()
        {
            Console.WriteLine("\n=== ПАКЕТНЫЙ ИМПОРТ КНИГ ===");
            Console.WriteLine("Введите книги в формате: Название;Автор;Жанр;Год;Цена");
            Console.WriteLine("Каждая книга на новой строке. Для завершения введите пустую строку.");
            Console.WriteLine("Доступные жанры: Фантастика, Детектив, Роман, Научная_литература, Поэзия, Детская_литература");
            
            var input = new List<string>();
            string line;
            
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                input.Add(line);
            }
            
            if (input.Count > 0)
            {
                _libraryManager.ImportBooksFromText(string.Join("\n", input));
            }
        }

        static void ShowShoppingCartMenu()
        {
            var cart = _libraryManager.GetShoppingCart();
            
            while (true)
            {
                Console.WriteLine("\n=== КОРЗИНА ===");
                Console.WriteLine("1. Показать корзину");
                Console.WriteLine("2. Добавить книгу в корзину");
                Console.WriteLine("3. Удалить книгу из корзины");
                Console.WriteLine("4. Очистить корзину");
                Console.WriteLine("5. Показать общую стоимость");
                Console.WriteLine("0. Назад в главное меню");
                Console.Write("Выберите действие: ");
                
                var choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        cart.DisplayCart();
                        break;
                    case "2":
                        AddBookToCart(cart);
                        break;
                    case "3":
                        RemoveBookFromCart(cart);
                        break;
                    case "4":
                        cart.Clear();
                        Console.WriteLine("Корзина очищена!");
                        break;
                    case "5":
                        Console.WriteLine($"Общая стоимость корзины: {cart.GetTotalPrice():C}");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
                
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void AddBookToCart(ShoppingCart cart)
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ КНИГИ В КОРЗИНУ ===");
            _libraryManager.DisplayAllBooks();
            
            Console.Write("Введите ID книги для добавления в корзину: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var book = _libraryManager.GetBookById(id);
                if (book != null)
                {
                    cart.AddBook(book);
                    Console.WriteLine("Книга добавлена в корзину!");
                }
                else
                {
                    Console.WriteLine("Книга с указанным ID не найдена!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }

        static void RemoveBookFromCart(ShoppingCart cart)
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ КНИГИ ИЗ КОРЗИНЫ ===");
            cart.DisplayCart();
            
            if (cart.GetItemCount() == 0)
            {
                Console.WriteLine("Корзина пуста!");
                return;
            }
            
            Console.Write("Введите ID книги для удаления из корзины: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var book = cart.GetItems().FirstOrDefault(b => b.Id == id);
                if (book != null)
                {
                    cart.RemoveBook(book);
                    Console.WriteLine("Книга удалена из корзины!");
                }
                else
                {
                    Console.WriteLine("Книга с указанным ID не найдена в корзине!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ID!");
            }
        }
    }
}
