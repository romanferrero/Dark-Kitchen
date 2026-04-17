using System.Security.Claims;
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

    [HttpPatch("{id}")]
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin, UserRole.Dispatcher)]
    public IActionResult UpdateStatus(int id, UpdateStatusEntryDTO actionDto)
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if(!Enum.TryParse<UserRole>(roleClaim, out var userRole))
        {
            return Unauthorized();
        }

        var allowed = actionDto.Action switch
        {
            "Prepared" => userRole is UserRole.Dispatcher or UserRole.Admin,
            "Cancel" => userRole is UserRole.Admin,
            "OnTheWay" => userRole is UserRole.Dispatcher,
            "Delivered" => userRole is UserRole.Dispatcher,
            "NotDelivered" => userRole is UserRole.Dispatcher,
            _ => false
        };

        if(!allowed)
        {
            return Unauthorized();
        }

        try
        {
            var result = orderService.UpdateStatus(id, actionDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
