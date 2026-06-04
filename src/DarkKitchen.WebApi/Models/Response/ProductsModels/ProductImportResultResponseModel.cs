namespace DarkKitchen.WebApi.Models.Response.ProductsModels;

public class ProductImportResultResponseModel
{
    public int ImportedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
