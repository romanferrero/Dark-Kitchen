using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using DarkKitchen.WebApi.Models.Response.OrdersModels;

namespace DarkKitchen.WebApi.Mappers;

internal static class OrderMapper
{
    internal static CreateOrderEntryDto ToDto(CreateOrderRequestModel request)
    {
        var products = new List<OrderProductEntryDto>();
        foreach(var item in request.Products)
        {
            products.Add(new OrderProductEntryDto(item.ProductCode, item.Quantity));
        }

        return new CreateOrderEntryDto(request.ClientId,
            request.DeliveryType,
            request.Street,
            request.DoorNumber,
            request.Apartment,
            products);
    }

    internal static UpdateOrderStatusEntryDto ToDto(UpdateOrderStatusRequestModel request)
    {
        return new UpdateOrderStatusEntryDto(request.Action);
    }

    internal static CreateOrderResponseModel ToResponse(CreateOrderResultExitDto createOrder)
    {
        return new CreateOrderResponseModel
        {
            ClientId = createOrder.ClientId,
            OrderNumber = createOrder.OrderNumber,
            Subtotal = createOrder.Subtotal,
            ShippingCost = createOrder.ShippingCost,
            Tax = createOrder.Tax,
            Total = createOrder.Total
        };
    }

    internal static UpdateOrderStatusResponseModel ToResponse(UpdateStatusExitDto status)
    {
        return new UpdateOrderStatusResponseModel
        {
            Status = status.Status,
            UpdatedAt = status.UpdatedAt
        };
    }

    internal static OrderDetailResponseModel ToResponse(OrderDetailExitDto detail)
    {
        return new OrderDetailResponseModel
        {
            OrderId = detail.OrderId,
            OrderNumber = detail.OrderNumber,
            ClientId = detail.ClientId,
            ClientFullName = detail.ClientFullName,
            OrderDate = detail.OrderDate,
            Status = detail.Status,
            Subtotal = detail.Subtotal,
            ShippingCost = detail.ShippingCost,
            Tax = detail.Tax,
            TotalCost = detail.TotalCost,
            Products = [.. detail.Products.Select(p => new OrderProductDetailResponseModel
            {
                Code = p.Code,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                Quantity = p.Quantity,
                PromotionName = p.PromotionName,
                DiscountPercentage = p.DiscountPercentage
            })]
        };
    }

    internal static OrderSummaryResponseModel ToResponse(OrderSummaryExitDto order)
    {
        return new OrderSummaryResponseModel
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = order.ClientFullName,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalCost = order.TotalCost,
            ProductCount = order.ProductCount
        };
    }
}
