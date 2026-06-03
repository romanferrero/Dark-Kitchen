namespace DarkKitchen.IBusinessLogic.DTOs.Exit.ImportDTOs;

public class ImporterInfoDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ImporterParameterDto> Parameters { get; set; } = [];
}
