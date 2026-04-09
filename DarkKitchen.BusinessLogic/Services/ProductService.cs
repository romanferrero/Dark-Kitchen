using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public List<Product> GetProducts(string? line, List<string>? categories, string? name)
    {
        var products = productRepository.GetFiltered(line, categories, name);
        return products.Where(p => p.Active).ToList();
    }

    public string CreateProduct(int code, string name, string description,
                                string line, string category, string images, bool active)
    {
        throw new NotImplementedException();
    }

    public string UpdateProduct(int code, string name, string description,
                                string line, string category, string images, bool active)
    {
        throw new NotImplementedException();
    }
}
