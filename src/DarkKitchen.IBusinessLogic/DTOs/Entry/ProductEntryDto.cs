namespace DarkKitchen.IBusinessLogic.DTOs.Entry;

public record ProductEntryDto(
    string Name,
    decimal Price,
    string Description,
    string Line,
    string Category,
    string Images,
    bool Active
);
