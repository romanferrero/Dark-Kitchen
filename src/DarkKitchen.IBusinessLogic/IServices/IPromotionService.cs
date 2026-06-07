using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IPromotionService
{
    PromotionExitDto CreatePromotion(CreatePromotionEntryDto dto, string responsibleUser);

    PromotionExitDto UpdatePromotion(UpdatePromotionEntryDto dto, string responsibleUser);

    ProductExitDto AddProduct(int promotionId, string productCode, string responsibleUser);

    ProductExitDto RemoveProduct(int promotionId, string productCode, string responsibleUser);

    List<PromotionExitDto> GetPromotions(DateOnly? date, string? line, string? product);
}
