using System;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Genre Genre { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }

        public Book(string title, string author, Genre genre, int year, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название книги не может быть пустым");
            
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Автор не может быть пустым");
            
            if (year < 0 || year > DateTime.Now.Year + 1)
                throw new ArgumentException("Год издания должен быть положительным и не больше текущего года + 1");
            
            if (price < 0)
                throw new ArgumentException("Цена не может быть отрицательной");

            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Название: {Title}, Автор: {Author}, Жанр: {Genre}, Год: {Year}, Цена: {Price:C}";
        }
    }
}
