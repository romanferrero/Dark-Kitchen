using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(UserRole.Client)]
    public IActionResult CreateOrder(CreateOrderRequestModel request)
    {
        var result = orderService.CreateOrder(
            request.ClientId,
            request.DeliveryType,
            request.Street,
            request.DoorNumber,
            request.Apartment,
            request.Products);

        var response = new CreateOrderResponseModel
        {
            ClientId = result.ClientId,
            OrderNumber = result.OrderNumber,
            Subtotal = result.Subtotal,
            ShippingCost = result.ShippingCost,
            Total = result.Total,
        };

        return Created(string.Empty, response);
    }

    [HttpPatch("{id}/status")]
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin)]
    public IActionResult UpdateStatus(int id, UpdateOrderStatusRequestModel request)
    {
        orderService.UpdateStatus(id, request.Status);
        return Ok();
    }
}
