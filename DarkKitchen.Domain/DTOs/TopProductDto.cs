namespace DarkKitchen.Domain.DTOs;

public class TopProductDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public List<string> ImageUrls { get; set; } = [];
}
