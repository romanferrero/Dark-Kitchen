using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;

namespace DarkKitchen.WebApi.Mappers;

internal static class ProductMapper
{
    internal static ProductEntryDto ToDto(ProductRequestModel request)
    {
        return new ProductEntryDto(
            request.Name,
            request.Price,
            request.Description,
            request.Line,
            request.Category,
            request.Images,
            request.Active);
    }

    internal static ProductResponseModel ToResponse(ProductExitDto product)
    {
        return new ProductResponseModel
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Line = product.Line,
            Category = product.Category,
            Active = product.Active,
            ImageUrls = product.ImageUrls
        };
    }
}
