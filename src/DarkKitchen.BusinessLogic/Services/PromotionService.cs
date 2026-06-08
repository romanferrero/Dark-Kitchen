using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class PromotionService(
    IPromotionRepository promotionRepository,
    IProductRepository productRepository,
    IAuditLogRepository auditLogRepository) : IPromotionService
{
    public PromotionExitDto CreatePromotion(CreatePromotionEntryDto dto, string responsibleUser)
    {
        var promotion = Promotion.Create(dto.Name, dto.Discount, dto.DateFrom, dto.DateTo);

        promotionRepository.Add(promotion);

        auditLogRepository.Add(AuditLog.Create("PROMOTION", promotion.Id, "Creation", responsibleUser));

        return ToExitDto(promotion);
    }

    public PromotionExitDto UpdatePromotion(UpdatePromotionEntryDto dto, string responsibleUser)
    {
        var promotion = promotionRepository.Get(p => p.Id == dto.Id)
                        ?? throw new KeyNotFoundException($"Promotion {dto.Id} not found");

        promotion.Update(dto.Name, dto.Discount, dto.DateFrom, dto.DateTo);

        promotionRepository.Update(promotion);

        auditLogRepository.Add(AuditLog.Create("PROMOTION", promotion.Id, "Modification", responsibleUser));

        return ToExitDto(promotion);
    }

    public ProductExitDto AddProduct(int promotionId, string productCode, string responsibleUser)
    {
        var promotion = promotionRepository.Get(p => p.Id == promotionId)
                        ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = productRepository.Get(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException($"Product {productCode} not found");

        promotion.AddProduct(product);
        promotionRepository.Update(promotion);

        auditLogRepository.Add(AuditLog.Create("PROMOTION", promotion.Id, $"Product association {productCode}", responsibleUser));

        return ToExitDto(product);
    }

    public ProductExitDto RemoveProduct(int promotionId, string productCode, string responsibleUser)
    {
        var promotion = promotionRepository.Get(p => p.Id == promotionId)
                        ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = promotion.Products.FirstOrDefault(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException($"Product {productCode} not found in promotion");

        promotion.RemoveProduct(productCode);
        promotionRepository.Update(promotion);

        auditLogRepository.Add(AuditLog.Create("PROMOTION", promotion.Id, $"Product removal {productCode}", responsibleUser));

        return ToExitDto(product);
    }

    public List<PromotionExitDto> GetPromotions(DateOnly? date, string? line, string? product)
    {
        return promotionRepository.GetFiltered()
            .Where(p => MatchesDate(p, date)
                        && HasProductInLine(p, line)
                        && HasProductMatching(p, product))
            .Select(ToExitDto)
            .ToList();
    }

    private static bool MatchesDate(Promotion promotion, DateOnly? date) =>
        !date.HasValue || (promotion.DateFrom <= date.Value && promotion.DateTo >= date.Value);

    private static bool HasProductInLine(Promotion promotion, string? line) =>
        string.IsNullOrEmpty(line) || promotion.Products.Any(p => p.Line == line);

    private static bool HasProductMatching(Promotion promotion, string? search) =>
        string.IsNullOrEmpty(search) || promotion.Products.Any(p =>
            p.Code == search ||
            p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

    private static PromotionExitDto ToExitDto(Promotion promotion)
    {
        return new PromotionExitDto
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            DateFrom = promotion.DateFrom,
            DateTo = promotion.DateTo,
            Products = [.. promotion.Products.Select(p => p.Code)]
        };
    }

    private static ProductExitDto ToExitDto(Product product)
    {
        return new ProductExitDto
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
