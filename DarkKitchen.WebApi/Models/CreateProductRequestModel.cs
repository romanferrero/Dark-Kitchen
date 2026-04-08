namespace DarkKitchen.WebApi.Models;

public class CreateProductRequestModel
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Images { get; set; } = string.Empty;
    public bool Active { get; set; }
}
