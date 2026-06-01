using DarkKitchen.IBusinessLogic.DTOs.Entry.DeliveryTypeDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.DeliveryTypeDTOs;
using DarkKitchen.WebApi.Models.Request.DeliveryTypesModels;
using DarkKitchen.WebApi.Models.Response.DeliveryTypesModels;

namespace DarkKitchen.WebApi.Mappers;

internal static class DeliveryTypeMapper
{
    internal static DeliveryTypeEntryDto ToDto(DeliveryTypeRequestModel request)
        => new(request.Name, request.ShippingCost);

    internal static DeliveryTypeResponseModel ToResponse(DeliveryTypeExitDto dto)
        => new() { Id = dto.Id, Name = dto.Name, ShippingCost = dto.ShippingCost };
}
