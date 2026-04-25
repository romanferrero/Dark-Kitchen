using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IPromotionService
{
    PromotionExitDTO CreatePromotion(CreatePromotionEntryDto dto);

    PromotionExitDTO UpdatePromotion(UpdatePromotionEntryDto dto);

    ProductExitDTO AddProduct(int promotionId, string productCode);

    ProductExitDTO RemoveProduct(int promotionId, string productCode);

    List<PromotionExitDTO> GetPromotions(DateOnly? date, string? line, string? product);
}
