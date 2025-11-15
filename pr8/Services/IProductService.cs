using pr8.Models;

namespace pr8.Services;

public interface IProductService
{
    List<Product> GetAllProducts();
    Product? GetProductById(int id);
}

