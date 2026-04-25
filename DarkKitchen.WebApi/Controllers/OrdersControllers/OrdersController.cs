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
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(UserRole.Client)]
    public IActionResult CreateOrder(CreateOrderRequestModel request)
    {
        var result = orderService.CreateOrder(ToDto(request));

        return Created(string.Empty, ToResponse(result));
    }

    [HttpGet("{id:int}")]
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin)]
    public IActionResult GetOrderById(int id)
    {
        return Ok(ToResponse(orderService.GetOrderById(id)));
    }

    [HttpPatch("{id}")]
    [AuthorizationFilter(UserRole.Dispatcher, UserRole.Admin)]
    [OrderActionAuthorizationFilter]
    public IActionResult UpdateStatus(int id, UpdateOrderStatusRequestModel request)
    {
        var exit = orderService.UpdateStatus(id, new UpdateOrderStatusEntryDTO(request.Action));

        return Ok(ToResponse(exit));
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

    private CreateOrderEntryDto ToDto(CreateOrderRequestModel request)
    {
        return new CreateOrderEntryDto(request.ClientId,
            request.DeliveryType,
            request.Street,
            request.DoorNumber,
            request.Apartment,
            request.Products);
    }

    private static CreateOrderResponseModel ToResponse(CreateOrderResultExitDto createOrder)
    {
        return new CreateOrderResponseModel
        {
            ClientId = createOrder.ClientId,
            OrderNumber = createOrder.OrderNumber,
            Subtotal = createOrder.Subtotal,
            ShippingCost = createOrder.ShippingCost,
            Total = createOrder.Total
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
