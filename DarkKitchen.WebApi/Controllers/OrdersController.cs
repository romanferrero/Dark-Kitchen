using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder(CreateOrderRequestModel request)
    {
        try
        {
            var items = request.Items
                .Select(i => (i.ProductCode, i.Quantity))
                .ToList();

            var result = orderService.CreateOrder(
                request.ClientId,
                request.DeliveryType,
                request.Street,
                request.DoorNumber,
                request.Apartment,
                items);

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
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
