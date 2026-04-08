namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IPromotionService
{
    string CreatePromotion(string name, int discount, DateOnly dateFrom, DateOnly dateTo);

    IEnumerable<string> GetPromotions(string? date, string? line, string? product);
}
