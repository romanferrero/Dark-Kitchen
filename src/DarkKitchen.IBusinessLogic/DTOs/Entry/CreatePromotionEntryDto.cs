namespace DarkKitchen.IBusinessLogic.DTOs.Entry;

public record CreatePromotionEntryDto(
    string Name,
    decimal Discount,
    DateOnly DateFrom,
    DateOnly DateTo
);
