namespace DarkKitchen.WebApi.Models.Response.ProductsModels;

public class ImporterParameterResponseModel
{
    public string Name { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool Required { get; set; }
}
