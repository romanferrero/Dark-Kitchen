namespace DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;

public record CreateOrderResultExitDto(
    int ClientId,
    int OrderNumber,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Total
);
