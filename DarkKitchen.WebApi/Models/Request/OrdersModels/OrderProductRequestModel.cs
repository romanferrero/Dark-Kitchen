namespace DarkKitchen.WebApi.Models.Request.OrdersModels;

public class OrderProductRequestModel
{
    public string ProductCode { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
