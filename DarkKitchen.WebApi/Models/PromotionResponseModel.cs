namespace DarkKitchen.WebApi.Models;

public class PromotionResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DiscountPercentage { get; set; }

    public DateOnly DateFrom { get; set; }

    public DateOnly DateTo { get; set; }

    public List<string> Products { get; set; } = [];
}
