using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IDiscounts;

namespace DarkKitchen.BusinessLogic.Discounts;

public class BestDiscountCalculator : IDiscountCalculator
{
    public decimal CalculatePrice(Product product, List<Promotion> activePromotions)
    {
        var bestDiscount = activePromotions
            .Where(p => p.Products.Any(prod => prod.Code == product.Code))
            .Select(p => p.DiscountPercentage)
            .DefaultIfEmpty(0)
            .Max();

        return product.Price * (1 - (bestDiscount / 100m));
    }
}
