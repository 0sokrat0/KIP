using System;
using System.Collections.Generic;
using System.Linq;
using LibraryApp.Models;

namespace LibraryApp.Services
{
    public class LibraryManager
    {
        private List<Book> _books;
        private int _nextId;
        private ShoppingCart _shoppingCart;

        public LibraryManager()
        {
            _books = new List<Book>();
            _nextId = 1;
            _shoppingCart = new ShoppingCart();
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            var testBooks = new[]
            {
                new Book("Война и мир", "Лев Толстой", Genre.Роман, 1869, 1500),
                new Book("1984", "Джордж Оруэлл", Genre.Фантастика, 1949, 800),
                new Book("Убийство в Восточном экспрессе", "Агата Кристи", Genre.Детектив, 1934, 1200),
                new Book("Краткая история времени", "Стивен Хокинг", Genre.Научная_литература, 1988, 1000),
                new Book("Евгений Онегин", "Александр Пушкин", Genre.Поэзия, 1833, 600)
            };

            foreach (var book in testBooks)
            {
                book.Id = _nextId++;
                _books.Add(book);
            }
        }

        public void AddBook(string title, string author, Genre genre, int year, decimal price)
        {
            try
            {
                var book = new Book(title, author, genre, year, price);
                book.Id = _nextId++;
                _books.Add(book);
                Console.WriteLine("Книга успешно добавлена!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        public bool RemoveBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
                Console.WriteLine("Книга успешно удалена!");
                return true;
            }
            Console.WriteLine("Книга с указанным ID не найдена!");
            return false;
        }

        public List<Book> SearchByTitle(string title)
        {
            return _books.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Book> SearchByAuthor(string author)
        {
            return _books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Book> SearchByGenre(Genre genre)
        {
            return _books.Where(b => b.Genre == genre).ToList();
        }

        public List<Book> SortByTitle()
        {
            return _books.OrderBy(b => b.Title).ToList();
        }

        public List<Book> SortByYear()
        {
            return _books.OrderBy(b => b.Year).ToList();
        }

        public Book GetMostExpensiveBook()
        {
            return _books.OrderByDescending(b => b.Price).FirstOrDefault();
        }

        public Book GetCheapestBook()
        {
            return _books.OrderBy(b => b.Price).FirstOrDefault();
        }

        public Dictionary<string, int> GetBooksByAuthor()
        {
            return _books.GroupBy(b => b.Author)
                        .ToDictionary(g => g.Key, g => g.Count());
        }

        public void DisplayBooks(List<Book> books)
        {
            if (books.Count == 0)
            {
                Console.WriteLine("Книги не найдены");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine(book);
            }
        }

        public void DisplayAllBooks()
        {
            Console.WriteLine("=== ВСЕ КНИГИ ===");
            DisplayBooks(_books);
        }

        public void ImportBooksFromText(string text)
        {
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int importedCount = 0;

            foreach (var line in lines)
            {
                try
                {
                    var parts = line.Split(';');
                    if (parts.Length != 5)
                    {
                        Console.WriteLine($"Неверный формат строки: {line}");
                        continue;
                    }

                    var title = parts[0].Trim();
                    var author = parts[1].Trim();
                    
                    if (!Enum.TryParse<Genre>(parts[2].Trim(), out var genre))
                    {
                        Console.WriteLine($"Неверный жанр в строке: {line}");
                        continue;
                    }

                    if (!int.TryParse(parts[3].Trim(), out var year))
                    {
                        Console.WriteLine($"Неверный год в строке: {line}");
                        continue;
                    }

                    if (!decimal.TryParse(parts[4].Trim(), out var price))
                    {
                        Console.WriteLine($"Неверная цена в строке: {line}");
                        continue;
                    }

                    var book = new Book(title, author, genre, year, price);
                    book.Id = _nextId++;
                    _books.Add(book);
                    importedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при импорте строки '{line}': {ex.Message}");
                }
            }

            Console.WriteLine($"Импортировано книг: {importedCount}");
        }

        public ShoppingCart GetShoppingCart()
        {
            return _shoppingCart;
        }

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }
    }
}
