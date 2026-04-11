using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository) : IPromotionService
{
    public string CreatePromotion(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        var promotion = Promotion.Create(name, discountPercentage, dateFrom, dateTo);
        promotionRepository.Add(promotion);
        return "Promotion created successfully.";
    }

    public string UpdatePromotion(int id, string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        var promotion = promotionRepository.GetById(id)
            ?? throw new KeyNotFoundException($"Promotion with id '{id}' not found.");

        promotion.Update(name, discountPercentage, dateFrom, dateTo);
        promotionRepository.Update(promotion);
        return "Promotion updated successfully.";
    }

    public string AddProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.GetById(promotionId)
            ?? throw new KeyNotFoundException($"Promotion with id '{promotionId}' not found.");

        var product = productRepository.GetByCode(productCode)
            ?? throw new KeyNotFoundException($"Product with code '{productCode}' not found.");

        promotion.AddProduct(product);
        promotionRepository.Update(promotion);
        return "Product added to promotion successfully.";
    }

    public string RemoveProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.GetById(promotionId)
            ?? throw new KeyNotFoundException($"Promotion with id '{promotionId}' not found.");

        promotion.RemoveProduct(productCode);
        promotionRepository.Update(promotion);
        return "Product removed from promotion successfully.";
    }

    public List<Promotion> GetPromotions(DateOnly? date, string? line, string? product)
    {
        throw new NotImplementedException();
    }
}
