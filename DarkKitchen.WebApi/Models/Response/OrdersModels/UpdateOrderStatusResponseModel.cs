namespace DarkKitchen.WebApi.Models.Response.OrdersModels;

public class UpdateOrderStatusResponseModel
{
    public string Status { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
}
