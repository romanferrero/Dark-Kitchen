using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

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
        var promotion = promotionRepository.GetAll(p => p.Id == id).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Promotion with id '{id}' not found.");

        promotion.Update(name, discountPercentage, dateFrom, dateTo);
        promotionRepository.Update(promotion);
        return "Promotion updated successfully.";
    }

    public string AddProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.GetAll(p => p.Id == promotionId).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Promotion with id '{promotionId}' not found.");

        var product = productRepository.GetByCode(productCode)
            ?? throw new KeyNotFoundException($"Product with code '{productCode}' not found.");

        promotion.AddProduct(product);
        promotionRepository.Update(promotion);
        return "Product added to promotion successfully.";
    }

    public string RemoveProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.GetAll(p => p.Id == promotionId).FirstOrDefault()
            ?? throw new KeyNotFoundException($"Promotion with id '{promotionId}' not found.");

        promotion.RemoveProduct(productCode);
        promotionRepository.Update(promotion);
        return "Product removed from promotion successfully.";
    }

    public List<Promotion> GetPromotions(DateOnly? date, string? line, string? product)
    {
        return promotionRepository.GetFiltered(date, line, product);
    }
}
