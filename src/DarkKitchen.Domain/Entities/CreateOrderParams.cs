namespace DarkKitchen.Domain.Entities;

public record CreateOrderParams(
    string DeliveryName,
    Address Address,
    List<OrderProduct> Products,
    int ClientId,
    int OrderNumber,
    decimal Subtotal,
    decimal ShippingCost,
    decimal TotalCost);
