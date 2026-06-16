namespace DarkKitchen.WebApi.Models.Request.DeliveryTypesModels;

public class DeliveryTypeRequestModel
{
    public string Name { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }
}
