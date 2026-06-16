namespace DarkKitchen.WebApi.Models;

public class GetProductsQueryModel
{
    public string? Line { get; set; }
    public string? Categories { get; set; }
    public string? Name { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
