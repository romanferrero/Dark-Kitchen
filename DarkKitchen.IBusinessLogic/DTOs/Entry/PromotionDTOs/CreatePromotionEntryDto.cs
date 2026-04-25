namespace DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;

public record CreatePromotionEntryDto(
    string Name,
    decimal Discount,
    DateOnly DateFrom,
    DateOnly DateTo
);
