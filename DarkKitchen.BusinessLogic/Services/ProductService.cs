using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public ProductExitDTO CreateProduct(ProductEntryDto dto)
    {
        var productCode = GenerateUniqueCode(c => productRepository.GetAll(p => p.Code == c).Any());
        var product = Product.Create(productCode, dto.Name, dto.Price, dto.Description, dto.Line, dto.Category,
            dto.Images, dto.Active);

        productRepository.Add(product);

        return ToExitDTO(product);
    }

    public ProductExitDTO UpdateProduct(string code, ProductEntryDto dto)
    {
        var product = productRepository.GetAll(p => p.Code == code).FirstOrDefault()
                      ?? throw new KeyNotFoundException($"Product {code} not found");

        product.Update(dto.Name, dto.Price, dto.Description, dto.Line, dto.Category, dto.Images, dto.Active);

        productRepository.Update(product);

        return ToExitDTO(product);
    }

    public List<ProductExitDTO> GetProducts(string? line, List<string>? categories, string? name)
    {
        var products = productRepository.GetFiltered(line, categories, name);
        return [.. products.Where(p => p.Active).Select(ToExitDTO)];
    }

    private static string GenerateUniqueCode(Func<string, bool> exists)
    {
        string code;
        do
        {
            code = $"PROD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
        while(exists(code));

        return code;
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
