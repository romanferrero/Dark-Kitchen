using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IDiscountCalculator
{
    decimal CalculatePrice(Product product, List<Promotion> activePromotions);
}
