using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public ProductExitDto CreateProduct(ProductEntryDto dto)
    {
        var productCode = GenerateUniqueCode(c => productRepository.Exists(p => p.Code == c));
        var product = Product.Create(productCode, dto.Name, dto.Price, dto.Description, dto.Line, dto.Category,
            dto.Images, dto.Active);

        productRepository.Add(product);

        return ToExitDTO(product);
    }

    public ProductExitDto UpdateProduct(int id, ProductEntryDto dto)
    {
        var product = productRepository.Get(p => p.Id == id)
                      ?? throw new KeyNotFoundException($"Product {id} not found");

        product.Update(dto.Name, dto.Price, dto.Description, dto.Line, dto.Category, dto.Images, dto.Active);

        productRepository.Update(product);

        return ToExitDTO(product);
    }

    public List<ProductExitDto> GetProducts(string? line, List<string>? categories, string? name)
    {
        var allProducts = productRepository.GetFiltered();

        var result = new List<ProductExitDto>();
        foreach(var product in allProducts)
        {
            if(!product.Active)
            {
                continue;
            }

            if(!MatchesLine(product, line))
            {
                continue;
            }

            if(!MatchesCategory(product, categories))
            {
                continue;
            }

            if(!MatchesName(product, name))
            {
                continue;
            }

            result.Add(ToExitDTO(product));
        }

        return result;
    }

    private static bool MatchesLine(Product product, string? line)
    {
        if(string.IsNullOrEmpty(line))
        {
            return true;
        }

        return product.Line == line;
    }

    private static bool MatchesCategory(Product product, List<string>? categories)
    {
        if(categories == null || categories.Count == 0)
        {
            return true;
        }

        return categories.Contains(product.Category);
    }

    private static bool MatchesName(Product product, string? name)
    {
        if(string.IsNullOrEmpty(name))
        {
            return true;
        }

        return product.Name.ToLower().Contains(name.ToLower());
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

    private static ProductExitDto ToExitDTO(Product product)
    {
        return new ProductExitDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Line = product.Line,
            Category = product.Category,
            ImageUrls = [.. product.Images.Select(i => i.Url)]
        };
    }
}
