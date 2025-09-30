using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryApp.Models
{
    public class ShoppingCart
    {
        private List<Book> _items;

        public ShoppingCart()
        {
            _items = new List<Book>();
        }

        public void AddBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            
            _items.Add(book);
        }

        public void RemoveBook(Book book)
        {
            _items.Remove(book);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public decimal GetTotalPrice()
        {
            return _items.Sum(book => book.Price);
        }

        public int GetItemCount()
        {
            return _items.Count;
        }

        public List<Book> GetItems()
        {
            return new List<Book>(_items);
        }

        public void DisplayCart()
        {
            if (_items.Count == 0)
            {
                Console.WriteLine("Корзина пуста");
                return;
            }

            Console.WriteLine("=== КОРЗИНА ===");
            foreach (var book in _items)
            {
                Console.WriteLine(book);
            }
            Console.WriteLine($"Общая стоимость: {GetTotalPrice():C}");
        }
    }
}
