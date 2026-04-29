namespace DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;

public record CreateOrderEntryDto(
    int ClientId,
    string DeliveryType,
    string Street,
    string DoorNumber,
    string Apartment,
    List<OrderProductEntryDto> Products
);
