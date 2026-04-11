using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public string CreateProduct(string code, string name, string description,
                            string line, string category, string images, bool active)
{
    var imageList = ParseImages(images);

    var product = Product.Create(code, name, description, line, category, imageList, active);

    productRepository.Add(product);
    return "Product created successfully.";
}

    public List<Product> GetProducts(string? line, List<string>? categories, string? name)
    {
        var products = productRepository.GetFiltered(line, categories, name);
        return products.Where(p => p.Active).ToList();
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
        var product = productRepository.GetByCode(code)
            ?? throw new KeyNotFoundException($"Product with code '{code}' not found.");

        if(name.Length < 10 || name.Length > 50)
        {
            throw new ArgumentException("Product name must be between 10 and 50 characters.");
        }

        if(description.Length < 20 || description.Length > 500)
        {
            throw new ArgumentException("Product description must be between 20 and 500 characters.");
        }

        var imageList = ParseImages(images);

        product.Name = name;
        product.Description = description;
        product.Line = line;
        product.Category = category;
        product.Images = imageList;
        product.Active = active;

        productRepository.Update(product);
        return "Product updated successfully.";
    }
}
