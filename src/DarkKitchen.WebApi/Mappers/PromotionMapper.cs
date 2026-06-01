using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.WebApi.Models.Request.PromotionsModels;
using DarkKitchen.WebApi.Models.Response.PromotionsModels;

namespace DarkKitchen.WebApi.Mappers;

internal static class PromotionMapper
{
    internal static CreatePromotionEntryDto ToDto(CreatePromotionRequestModel request)
    {
        return new CreatePromotionEntryDto(
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);
    }

    internal static UpdatePromotionEntryDto ToDto(int id, UpdatePromotionRequestModel request)
    {
        return new UpdatePromotionEntryDto(
            id,
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);
    }

    internal static PromotionResponseModel ToResponse(PromotionExitDto promotion)
    {
        return new PromotionResponseModel
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            DateFrom = promotion.DateFrom,
            DateTo = promotion.DateTo,
            Products = promotion.Products
        };
    }
}
