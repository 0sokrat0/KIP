using System;


namespace StoreManagement
{
    public class Product
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public bool IsInStock => Quantity > 0;
        public Category Category { get; private set; }
        
        public Product(string name, decimal price, int quantity, Category category)
        {
            ValidateInput(name, price, quantity);
            
            Name = name.Trim();
            Price = price;
            Quantity = quantity;
            Category = category;
            Code = GenerateCode();
        }
        
        private void ValidateInput(string name, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым.");
            
            if (price < 0)
                throw new ArgumentException("Цена не может быть отрицательной.");
            
            if (quantity < 0)
                throw new ArgumentException("Количество не может быть отрицательным.");
        }
        
        private string GenerateCode()
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            return myuuidAsString;
        }
        
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Цена не может быть отрицательной.");
            
            Price = newPrice;
        }
        
        public void AddQuantity(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Количество для добавления не может быть отрицательным.");
            
            Quantity += amount;
        }
        
        public bool TrySell(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Количество для продажи не может быть отрицательным.");
            
            if (Quantity >= amount)
            {
                Quantity -= amount;
                return true;
            }
            
            return false;
        }
        
        public override string ToString()
        {
            return $"Код: {Code}\n" +
                   $"Название: {Name}\n" +
                   $"Цена: {Price:C}\n" +
                   $"Количество: {Quantity}\n" +
                   $"На складе: {(IsInStock ? "Да" : "Нет")}\n" +
                   $"Категория: {CategoryHelper.GetCategoryName(Category)}";
        }
        
        public string GetShortInfo()
        {
            return $"{Code} - {Name} ({CategoryHelper.GetCategoryName(Category)}) - {Price:C} - Остаток: {Quantity}";
        }
    }
}
