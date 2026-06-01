namespace DarkKitchen.WebApi.Models.Response.DeliveryTypesModels;

public class DeliveryTypeResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }
}
