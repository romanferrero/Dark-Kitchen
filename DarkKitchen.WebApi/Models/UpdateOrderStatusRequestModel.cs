using DarkKitchen.Domain;

namespace DarkKitchen.WebApi.Models;

public class UpdateOrderStatusRequestModel
{
    public OrderStatus Status { get; set; }
}
