namespace DarkKitchen.WebApi.Models.Request.PromotionsModels;

public class CreatePromotionRequestModel
{
    public string Name { get; set; } = string.Empty;
    public int Discount { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
}
