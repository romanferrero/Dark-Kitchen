using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic.IDiscounts;

public interface IDiscountCalculator
{
    decimal CalculatePrice(Product product, List<Promotion> activePromotions);
}
