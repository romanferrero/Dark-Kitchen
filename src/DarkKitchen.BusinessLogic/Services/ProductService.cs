using DarkKitchen.BusinessLogic.Helpers;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class ProductService(IProductRepository productRepository, IAuditLogRepository auditLogRepository) : IProductService
{
    public ProductExitDto CreateProduct(ProductEntryDto dto, string responsibleUser)
    {
        var productCode = ProductCodeGenerator.GenerateUniqueCode(c => productRepository.Exists(p => p.Code == c));
        var product = Product.Create(new CreateProductParamsDto(productCode, dto.Name, dto.Price, dto.Description,
            dto.Line, dto.Category, dto.Images, dto.Active));

        productRepository.Add(product);

        auditLogRepository.Add(AuditLog.Create("PRODUCT", product.Id, "Creation", responsibleUser));

        return ToExitDto(product);
    }

    public ProductExitDto UpdateProduct(int id, ProductEntryDto dto, string responsibleUser)
    {
        var product = productRepository.Get(p => p.Id == id)
                      ?? throw new KeyNotFoundException($"Product {id} not found");

        product.Update(dto.Name, dto.Price, dto.Description, dto.Line, dto.Category, dto.Images, dto.Active);

        productRepository.Update(product);

        auditLogRepository.Add(AuditLog.Create("PRODUCT", product.Id, "Modification", responsibleUser));

        return ToExitDto(product);
    }

    public List<ProductExitDto> GetProducts(string? line, List<string>? categories, string? name)
    {
        return productRepository.GetFiltered()
            .Where(p => p.Active
                        && MatchesLine(p, line)
                        && MatchesCategory(p, categories)
                        && MatchesName(p, name))
            .Select(ToExitDto)
            .ToList();
    }

    private static bool MatchesLine(Product product, string? line) =>
        string.IsNullOrEmpty(line) || product.Line == line;

    private static bool MatchesCategory(Product product, List<string>? categories) =>
        categories is null || categories.Count == 0 || categories.Contains(product.Category);

    private static bool MatchesName(Product product, string? name) =>
        string.IsNullOrEmpty(name) || product.Name.Contains(name, StringComparison.OrdinalIgnoreCase);

    private static ProductExitDto ToExitDto(Product product)
    {
        return new ProductExitDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Line = product.Line,
            Category = product.Category,
            Active = product.Active,
            ImageUrls = [.. product.Images.Select(i => i.Url)]
        };
    }
}
