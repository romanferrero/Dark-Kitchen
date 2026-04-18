namespace DarkKitchen.IBusinessLogic;

public class OrderDetailDTO
{
    public int OrderNumber { get; set; }

    public int ClientId { get; set; }

    public string ClientFullName { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalCost { get; set; }

    public List<OrderProductDetailDTO> Products { get; set; } = [];
}
