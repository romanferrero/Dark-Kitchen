namespace DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;

public class CreateOrderResultExitDto
{
    public int ClientId { get; set; }

    public int OrderNumber { get; set; }

    public decimal Subtotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal Total { get; set; }
}
