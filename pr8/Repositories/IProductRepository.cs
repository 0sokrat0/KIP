using pr8.Models;

namespace pr8.Repositories;

public interface IProductRepository
{
    Product? GetById(int id);
    List<Product> GetAll();
    void Add(Product product);
    void Update(Product product);
    void Delete(int id);
}

