using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class PromotionService(IPromotionRepository promotionRepository) : IPromotionService
{
    public string CreatePromotion(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        var promotion = Promotion.Create(name, discountPercentage, dateFrom, dateTo);
        promotionRepository.Add(promotion);
        return "Promotion created successfully.";
    }

    public string UpdatePromotion(int id, string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        throw new NotImplementedException();
    }

    public string AddProduct(int promotionId, string productCode)
    {
        throw new NotImplementedException();
    }

    public string RemoveProduct(int promotionId, string productCode)
    {
        throw new NotImplementedException();
    }

    public List<Promotion> GetPromotions(DateOnly? date, string? line, string? product)
    {
        throw new NotImplementedException();
    }
}
