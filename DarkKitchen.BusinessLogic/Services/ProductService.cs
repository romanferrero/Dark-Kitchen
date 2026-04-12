using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

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
        var product = productRepository.GetByCode(code)
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
