namespace DarkKitchen.WebApi.Models.Request.OrdersModels;

public class CreateOrderRequestModel
{
    public int ClientId { get; set; }

    public string DeliveryType { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string DoorNumber { get; set; } = string.Empty;

    public string Apartment { get; set; } = string.Empty;

    public List<string> Products { get; set; } = [];
}
