namespace DarkKitchen.WebApi.Models.Request.ProductsModels;

public class ProductRequestModel
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0m;

    public string Description { get; set; } = string.Empty;

    public string Line { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Images { get; set; } = string.Empty;

    public bool Active { get; set; }
}
