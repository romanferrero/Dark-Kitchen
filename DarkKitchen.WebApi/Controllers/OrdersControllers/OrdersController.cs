using System.Security.Claims;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;
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

        return Created(string.Empty, ToResponse(result));
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

            return Ok(ToResponse(result));
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

        return Ok(orders.Select(ToResponse).ToList());
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

        return Ok(orders.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin)]
    public IActionResult GetOrderById(int id)
    {
        return Ok(ToResponse(orderService.GetOrderById(id)));
    }

    private static CreateOrderResponseModel ToResponse(OrderResultExitDTO order)
    {
        return new CreateOrderResponseModel
        {
            ClientId = order.ClientId,
            OrderNumber = order.OrderNumber,
            Subtotal = order.Subtotal,
            ShippingCost = order.ShippingCost,
            Total = order.Total
        };
    }

    private static UpdateOrderStatusResponseModel ToResponse(UpdateStatusExitDTO status)
    {
        return new UpdateOrderStatusResponseModel
        {
            Status = status.Status,
            UpdatedAt = status.UpdatedAt
        };
    }

    private static OrderSummaryResponseModel ToResponse(OrderSummaryExitDTO order)
    {
        return new OrderSummaryResponseModel
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = order.ClientFullName,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalCost = order.TotalCost,
            ProductCount = order.ProductCount
        };
    }

    private static OrderDetailResponseModel ToResponse(OrderDetailExitDTO detail)
    {
        return new OrderDetailResponseModel
        {
            OrderNumber = detail.OrderNumber,
            ClientId = detail.ClientId,
            ClientFullName = detail.ClientFullName,
            OrderDate = detail.OrderDate,
            Status = detail.Status,
            TotalCost = detail.TotalCost,
            Products = [.. detail.Products.Select(p => new OrderProductDetailResponseModel
            {
                Code = p.Code,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                PromotionName = p.PromotionName,
                DiscountPercentage = p.DiscountPercentage
            })]
        };
    }
}
