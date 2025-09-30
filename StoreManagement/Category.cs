using System;

namespace StoreManagement
{
    public enum Category
    {
        Electronics = 1,
        Clothing = 2,
        Food = 3,
        Books = 4,
        Sports = 5
    }
    
    public static class CategoryHelper
    {
        public static string GetCategoryName(Category category)
        {
            return category switch
            {
                Category.Electronics => "Электроника",
                Category.Clothing => "Одежда",
                Category.Food => "Продукты питания",
                Category.Books => "Книги",
                Category.Sports => "Спорт и отдых",
                _ => "Неизвестная категория"
            };
        }
        
        public static void ShowAllCategories()
        {
            Console.WriteLine("Доступные категории:");
            foreach (Category category in Enum.GetValues<Category>())
            {
                Console.WriteLine($"{(int)category}. {GetCategoryName(category)}");
            }
        }
        
        public static Category? GetCategoryByNumber(int number)
        {
            if (Enum.IsDefined(typeof(Category), number))
            {
                return (Category)number;
            }
            return null;
        }
    }
}
