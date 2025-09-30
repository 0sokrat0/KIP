using System;

namespace StoreManagement
{
    public class Sale
    {
        public string ProductCode { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int QuantitySold { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Category Category { get; private set; }
        
        public Sale(string productCode, string productName, decimal unitPrice, int quantitySold, Category category)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                throw new ArgumentException("Код товара не может быть пустым.");
            
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Название товара не может быть пустым.");
            
            if (unitPrice < 0)
                throw new ArgumentException("Цена за единицу не может быть отрицательной.");
            
            if (quantitySold <= 0)
                throw new ArgumentException("Количество проданных товаров должно быть положительным.");
            
            ProductCode = productCode;
            ProductName = productName;
            UnitPrice = unitPrice;
            QuantitySold = quantitySold;
            TotalAmount = unitPrice * quantitySold;
            SaleDate = DateTime.Now;
            Category = category;
        }
        
        public override string ToString()
        {
            return $"Дата: {SaleDate:dd.MM.yyyy HH:mm}\n" +
                   $"Код: {ProductCode}\n" +
                   $"Товар: {ProductName}\n" +
                   $"Категория: {CategoryHelper.GetCategoryName(Category)}\n" +
                   $"Цена за единицу: {UnitPrice:C}\n" +
                   $"Количество: {QuantitySold}\n" +
                   $"Общая сумма: {TotalAmount:C}";
        }
        
        public string GetShortInfo()
        {
            return $"{SaleDate:dd.MM.yyyy} - {ProductName} - {QuantitySold} шт. - {TotalAmount:C}";
        }
    }
}

