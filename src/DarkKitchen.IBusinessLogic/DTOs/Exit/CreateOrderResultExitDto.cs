namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public record CreateOrderResultExitDto(
    int ClientId,
    int OrderNumber,
    decimal Subtotal,
    decimal ShippingCost,
    decimal Tax,
    decimal Total
);
