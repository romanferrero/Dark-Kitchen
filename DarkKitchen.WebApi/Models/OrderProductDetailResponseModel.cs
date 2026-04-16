namespace DarkKitchen.WebApi.Models;

public class OrderProductDetailResponseModel
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? PromotionName { get; set; }

    public int? DiscountPercentage { get; set; }
}
