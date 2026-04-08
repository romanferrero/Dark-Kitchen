namespace DarkKitchen.WebApi.Models;
public class UpdateProductRequestModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Images { get; set; } = string.Empty;
    public bool Active { get; set; }
}
