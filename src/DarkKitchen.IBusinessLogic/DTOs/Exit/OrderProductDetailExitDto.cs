namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public class OrderProductDetailExitDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? PromotionName { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public int Quantity { get; set; }
}
