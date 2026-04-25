namespace DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;

public class ProductExitDTO
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Line { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = [];
}
