using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public ProductExitDTO CreateProduct(string code, string name, string description,
                            string line, string category, string images, bool active)
    {
        var product = Product.Create(code, name, description, line, category, images, active);
        productRepository.Add(product);
        return ToExitDTO(product);
    }

    public ProductExitDTO UpdateProduct(string code, string name, string description,
                                string line, string category, string images, bool active)
    {
        var product = productRepository.GetAll(p => p.Code == code).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Product {code} not found");

        product.Update(name, description, line, category, images, active);
        productRepository.Update(product);
        return ToExitDTO(product);
    }

    public List<ProductExitDTO> GetProducts(string? line, List<string>? categories, string? name)
    {
        var products = productRepository.GetFiltered(line, categories, name);
        return [.. products.Where(p => p.Active).Select(ToExitDTO)];
    }

    private static ProductExitDTO ToExitDTO(Product product)
    {
        return new ProductExitDTO
        {
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Line = product.Line,
            Category = product.Category,
            ImageUrls = [.. product.Images.Select(i => i.Url)]
        };
    }
}
