using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IPromotionService
{
    string CreatePromotion(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo);

    string UpdatePromotion(int id, string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo);

    string AddProduct(int promotionId, string productCode);

    string RemoveProduct(int promotionId, string productCode);

    List<Promotion> GetPromotions(DateOnly? date, string? line, string? product);
}
