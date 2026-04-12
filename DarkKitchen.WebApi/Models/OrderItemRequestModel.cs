namespace DarkKitchen.WebApi.Models;

public class OrderItemRequestModel
{
    public string ProductCode { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
