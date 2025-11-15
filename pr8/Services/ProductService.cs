using pr8.Models;
using pr8.Repositories;

namespace pr8.Services;

public class ProductService : IProductService
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
}

