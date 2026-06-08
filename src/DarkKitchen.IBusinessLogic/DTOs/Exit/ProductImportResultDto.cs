namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public class ProductImportResultDto
{
    public int ImportedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
