namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public class OrderDetailExitDto
{
    public int OrderId { get; set; }

    public int OrderNumber { get; set; }

    public int ClientId { get; set; }

    public string ClientFullName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal Tax { get; set; }

    public decimal TotalCost { get; set; }

    public List<OrderProductDetailExitDto> Products { get; set; } = [];
}
