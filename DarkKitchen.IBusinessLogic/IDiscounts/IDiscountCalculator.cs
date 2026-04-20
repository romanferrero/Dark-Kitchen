using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IBusinessLogic.IDiscounts;

public interface IDiscountCalculator
{
    decimal CalculatePrice(Product product, List<Promotion> activePromotions);
}
