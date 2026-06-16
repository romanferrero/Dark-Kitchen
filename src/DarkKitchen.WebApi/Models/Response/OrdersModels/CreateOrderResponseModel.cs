namespace DarkKitchen.WebApi.Models.Response.OrdersModels;

public class CreateOrderResponseModel
{
    public int ClientId { get; set; }

    public int OrderNumber { get; set; }

    public decimal Subtotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }
}
