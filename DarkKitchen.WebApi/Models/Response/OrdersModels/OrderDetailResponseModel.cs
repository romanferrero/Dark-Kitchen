namespace DarkKitchen.WebApi.Models.Response.OrdersModels;

public class OrderDetailResponseModel
{
    public int OrderNumber { get; set; }

    public int ClientId { get; set; }

    public string ClientFullName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalCost { get; set; }

    public List<OrderProductDetailResponseModel> Products { get; set; } = [];
}
