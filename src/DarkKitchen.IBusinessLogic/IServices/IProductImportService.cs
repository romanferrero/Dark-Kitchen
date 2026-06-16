using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IProductImportService
{
    List<ImporterInfoDto> GetAvailableImporters();

    ProductImportResultDto ImportProducts(ImportRequestDto request);
}
