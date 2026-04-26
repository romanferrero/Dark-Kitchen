using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IPromotionService
{
    PromotionExitDto CreatePromotion(CreatePromotionEntryDto dto);

    PromotionExitDto UpdatePromotion(UpdatePromotionEntryDto dto);

    ProductExitDto AddProduct(int promotionId, string productCode);

    ProductExitDto RemoveProduct(int promotionId, string productCode);

    List<PromotionExitDto> GetPromotions(DateOnly? date, string? line, string? product);
}
