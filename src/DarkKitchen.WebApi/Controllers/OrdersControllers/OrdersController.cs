using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.OrdersControllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilterAttribute(Permission.CreateOrder)]
    public IActionResult CreateOrder(CreateOrderRequestModel request)
    {
        var result = orderService.CreateOrder(OrderMapper.ToDto(request));

        return Created(string.Empty, OrderMapper.ToResponse(result));
    }

    [HttpPatch("{id}")]
    [AuthorizationFilterAttribute(Permission.UpdateOrderStatus)]
    [OrderActionAuthorizationFilterAttribute]
    public IActionResult UpdateStatus(int id, UpdateOrderStatusRequestModel request)
    {
        var exit = orderService.UpdateStatus(id, OrderMapper.ToDto(request));

        return Ok(OrderMapper.ToResponse(exit));
    }

    [HttpGet("{id:int}")]
    [AuthorizationFilterAttribute(Permission.ViewOrderDetail)]
    public IActionResult GetOrderById(int id)
    {
        return Ok(OrderMapper.ToResponse(orderService.GetOrderById(id)));
    }

    [HttpGet]
    [AuthorizationFilterAttribute(Permission.ListOrders)]
    public IActionResult GetOrders([FromQuery] GetOrdersQueryModel query)
    {
        var role = (string)HttpContext.Items["UserRole"]!;

        if(role == UserRole.Client.ToString())
        {
            var clientId = (int)HttpContext.Items["UserId"]!;
            var clientOrders = orderService.GetClientOrders(clientId, query.From, query.To, query.Status);
            return Ok(clientOrders.Select(OrderMapper.ToResponse).ToList());
        }

        if(!query.From.HasValue || !query.To.HasValue)
        {
            throw new ArgumentException("Date range (from and to) is required.");
        }

        var orders = orderService.GetDispatcherOrders(query.From.Value, query.To.Value, query.Street, query.Status);

        return Ok(orders.Select(OrderMapper.ToResponse).ToList());
    }
}
