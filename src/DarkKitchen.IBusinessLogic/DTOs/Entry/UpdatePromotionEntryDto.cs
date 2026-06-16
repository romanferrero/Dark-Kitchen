namespace DarkKitchen.IBusinessLogic.DTOs.Entry;

public record UpdatePromotionEntryDto(
    int Id,
    string Name,
    int Discount,
    DateOnly DateFrom,
    DateOnly DateTo
);
