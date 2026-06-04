namespace DarkKitchen.WebApi.Models.Request.ProductsModels;

public class ImportRequestModel
{
    public string ImporterName { get; set; } = string.Empty;

    public Dictionary<string, string> Parameters { get; set; } = [];
}
