namespace DarkKitchen.WebApi.Models.Response.PromotionsModels;

public class PromotionResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal DiscountPercentage { get; set; }

    public DateOnly DateFrom { get; set; }

    public DateOnly DateTo { get; set; }

    public List<string> Products { get; set; } = [];
}
