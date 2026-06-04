namespace DarkKitchen.IBusinessLogic.DTOs.Exit.ImportDTOs;

public class ProductImportResultDto
{
    public int ImportedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
