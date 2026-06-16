namespace DarkKitchen.WebApi.Models.Response.ProductsModels;

public class ImporterInfoResponseModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ImporterParameterResponseModel> Parameters { get; set; } = [];
}
