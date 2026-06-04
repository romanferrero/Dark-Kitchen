using DarkKitchen.IBusinessLogic.DTOs.Entry.ImportDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ImportDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IProductImportService
{
    List<ImporterInfoDto> GetAvailableImporters();

    ProductImportResultDto ImportProducts(ImportRequestDto request);
}
