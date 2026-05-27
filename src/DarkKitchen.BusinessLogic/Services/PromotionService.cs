using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository)
    : IPromotionService
{
    public PromotionExitDto CreatePromotion(CreatePromotionEntryDto dto)
    {
        var promotion = Promotion.Create(dto.Name, dto.Discount, dto.DateFrom, dto.DateTo);

        promotionRepository.Add(promotion);

        return ToExitDTO(promotion);
    }

    public PromotionExitDto UpdatePromotion(UpdatePromotionEntryDto dto)
    {
        var promotion = promotionRepository.Get(p => p.Id == dto.Id)
                        ?? throw new KeyNotFoundException($"Promotion {dto.Id} not found");

        promotion.Update(dto.Name, dto.Discount, dto.DateFrom, dto.DateTo);

        promotionRepository.Update(promotion);

        return ToExitDTO(promotion);
    }

    public ProductExitDto AddProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.Get(p => p.Id == promotionId)
                        ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = productRepository.Get(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException($"Product {productCode} not found");

        promotion.AddProduct(product);
        promotionRepository.Update(promotion);
        return ToExitDTO(product);
    }

    public ProductExitDto RemoveProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.Get(p => p.Id == promotionId)
                        ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = promotion.Products.FirstOrDefault(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException($"Product {productCode} not found in promotion");

        promotion.RemoveProduct(productCode);
        promotionRepository.Update(promotion);
        return ToExitDTO(product);
    }

    public List<PromotionExitDto> GetPromotions(DateOnly? date, string? line, string? product)
    {
        return promotionRepository.GetFiltered(date, line, product).Select(ToExitDTO).ToList();
    }

    private static PromotionExitDto ToExitDTO(Promotion promotion)
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

    private static ProductExitDto ToExitDTO(Product product)
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
