using DarkKitchen.IBusinessLogic.DTOs.Entry.ImportDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ImportDTOs;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;

namespace DarkKitchen.WebApi.Mappers;

internal static class ProductImportMapper
{
    internal static ImportRequestDto ToDto(ImportRequestModel request)
    {
        return new ImportRequestDto(request.ImporterName, request.Parameters);
    }

    internal static ImporterInfoResponseModel ToResponse(ImporterInfoDto dto)
    {
        return new ImporterInfoResponseModel
        {
            Name = dto.Name,
            Description = dto.Description,
            Parameters = dto.Parameters.Select(ToResponse).ToList()
        };
    }

    internal static ProductImportResultResponseModel ToResponse(ProductImportResultDto dto)
    {
        return new ProductImportResultResponseModel
        {
            ImportedCount = dto.ImportedCount,
            Errors = dto.Errors
        };
    }

    private static ImporterParameterResponseModel ToResponse(ImporterParameterDto dto)
    {
        return new ImporterParameterResponseModel
        {
            Name = dto.Name,
            Label = dto.Label,
            Description = dto.Description,
            Required = dto.Required
        };
    }
}
