namespace DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;

public class OrderProductDetailExitDTO
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? PromotionName { get; set; }

    public decimal? DiscountPercentage { get; set; }
}
