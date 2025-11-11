using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public List<Product> GetAllProducts()
    {
        return _productRepository.GetAll();
    }
    
    public Product? GetProductById(int id)
    {
        return _productRepository.GetById(id);
    }
    
    public void DisplayProducts(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("Товары не найдены");
            return;
        }
        
        Console.WriteLine("СПИСОК ТОВАРОВ");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"ID",-5} {"Название",-30} {"Цена",-15} {"Описание"}");
        Console.WriteLine(new string('-', 80));
        
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id,-5} {product.Name,-30} {product.Price,-15:F2} руб. {product.Description}");
        }
        
        Console.WriteLine(new string('-', 80));
    }
}
