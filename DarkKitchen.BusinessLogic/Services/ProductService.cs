using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public string CreateProduct(string code, string name, string description,
                            string line, string category, string images, bool active)
    {
        var product = Product.Create(code, name, description, line, category, images, active);
        productRepository.Add(product);
        return "Product created successfully.";
    }

    public string UpdateProduct(string code, string name, string description,
                                string line, string category, string images, bool active)
    {
        var product = productRepository.GetAll(p => p.Code == code).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Product with code '{code}' not found.");

        product.Update(name, description, line, category, images, active);

        productRepository.Update(product);
        return "Product updated successfully.";
    }

    public List<Product> GetProducts(string? line, List<string>? categories, string? name)
    {
        var products = productRepository.GetFiltered(line, categories, name);
        return products.Where(p => p.Active).ToList();
    }
}
