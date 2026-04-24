using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository) : IPromotionService
{
    public PromotionExitDTO CreatePromotion(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        var promotion = Promotion.Create(name, discountPercentage, dateFrom, dateTo);
        promotionRepository.Add(promotion);
        return ToExitDTO(promotion);
    }

    public PromotionExitDTO UpdatePromotion(int id, string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        var promotion = promotionRepository.GetAll(p => p.Id == id).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Promotion {id} not found");

        promotion.Update(name, discountPercentage, dateFrom, dateTo);
        promotionRepository.Update(promotion);
        return ToExitDTO(promotion);
    }

    public ProductExitDTO AddProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.GetAll(p => p.Id == promotionId).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = productRepository.GetAll(p => p.Code == productCode).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Product {productCode} not found");

        promotion.AddProduct(product);
        promotionRepository.Update(promotion);
        return ToExitDTO(product);
    }

    public ProductExitDTO RemoveProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.GetAll(p => p.Id == promotionId).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = promotion.Products.FirstOrDefault(p => p.Code == productCode)
            ?? throw new KeyNotFoundException($"Product {productCode} not found in promotion");

        promotion.RemoveProduct(productCode);
        promotionRepository.Update(promotion);
        return ToExitDTO(product);
    }

    public List<PromotionExitDTO> GetPromotions(DateOnly? date, string? line, string? product)
    {
        return promotionRepository.GetFiltered(date, line, product).Select(ToExitDTO).ToList();
    }

    private static PromotionExitDTO ToExitDTO(Promotion promotion)
    {
        return new PromotionExitDTO
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            DateFrom = promotion.DateFrom,
            DateTo = promotion.DateTo,
            Products = [.. promotion.Products.Select(p => p.Code)]
        };
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
