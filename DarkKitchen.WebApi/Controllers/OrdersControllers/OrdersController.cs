using System.Security.Claims;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using DarkKitchen.WebApi.Models.Response.OrdersModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.OrdersControllers;

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
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin)]
    public IActionResult UpdateStatus(int id, UpdateOrderStatusRequestModel request)
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        if(!Enum.TryParse<UserRole>(roleClaim, out var userRole))
        {
            return Unauthorized();
        }

        var allowed = request.Action switch
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
            var dto = new UpdateOrderStatusEntryDTO(request.Action);
            var result = orderService.UpdateStatus(id, dto);

            var response = new UpdateOrderStatusResponseModel { Status = result.Status, UpdatedAt = result.UpdatedAt };

            return Ok(response);
        }
        catch(KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    [AuthorizationFilter(UserRole.Client)]
    public IActionResult GetClientOrders([FromQuery] GetOrdersQueryModel query)
    {
        var clientId = (int)HttpContext.Items["UserId"]!;

        var orders = orderService.GetClientOrders(clientId, query.From, query.To, query.Status);

        var response = orders.Select(o => new OrderSummaryResponseModel
        {
            OrderNumber = o.OrderNumber,
            ClientId = o.ClientId,
            ClientFullName = o.ClientFullName,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalCost = o.TotalCost,
            ProductCount = o.ProductCount
        }).ToList();

        return Ok(response);
    }

    [HttpGet("dispatcher")]
    [AuthorizationFilter(UserRole.Dispatcher)]
    public IActionResult GetDispatcherOrders([FromQuery] GetOrdersQueryModel query)
    {
        if(!query.From.HasValue || !query.To.HasValue)
        {
            throw new ArgumentException("Date range (from and to) is required.");
        }

        var orders = orderService.GetDispatcherOrders(query.From.Value, query.To.Value, query.Street, query.Status);

        var response = orders.Select(o => new OrderSummaryResponseModel
        {
            OrderNumber = o.OrderNumber,
            ClientId = o.ClientId,
            ClientFullName = o.ClientFullName,
            OrderDate = o.OrderDate,
            Status = o.Status,
            TotalCost = o.TotalCost,
            ProductCount = o.ProductCount
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin)]
    public IActionResult GetOrderById(int id)
    {
        var detail = orderService.GetOrderById(id);

        var response = new OrderDetailResponseModel
        {
            OrderNumber = detail.OrderNumber,
            ClientId = detail.ClientId,
            ClientFullName = detail.ClientFullName,
            OrderDate = detail.OrderDate,
            Status = detail.Status,
            TotalCost = detail.TotalCost,
            Products = detail.Products.Select(p => new OrderProductDetailResponseModel
            {
                Code = p.Code,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                PromotionName = p.PromotionName,
                DiscountPercentage = p.DiscountPercentage
            }).ToList()
        };

        return Ok(response);
    }
}
