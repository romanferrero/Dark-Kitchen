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

    public string CreateProduct(string code, string name, string description,
                                string line, string category, string images, bool active)
    {
        if(code.Length < 5 || code.Length > 20)
            throw new ArgumentException("Product code must be between 5 and 20 characters.");

        if(name.Length < 10 || name.Length > 50)
            throw new ArgumentException("Product name must be between 10 and 50 characters.");

        var imageList = ParseImages(images);

        var product = new Product
        {
            Code = code,
            Name = name,
            Description = description,
            Line = line,
            Category = category,
            Images = imageList,
            Active = active,
        };

        productRepository.Add(product);
        return "Product created successfully.";
    }

    private static List<ProductImage> ParseImages(string images)
    {
        return images
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(url => new ProductImage { Url = url.Trim() })
            .ToList();
    }

    public string UpdateProduct(string code, string name, string description,
                                string line, string category, string images, bool active)
    {
        throw new NotImplementedException();
    }
}
