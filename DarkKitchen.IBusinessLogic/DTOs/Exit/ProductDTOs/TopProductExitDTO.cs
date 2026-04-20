namespace DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;

public class TopProductExitDTO
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public List<string> ImageUrls { get; set; } = [];
}
