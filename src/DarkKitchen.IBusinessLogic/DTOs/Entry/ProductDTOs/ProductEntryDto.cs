namespace DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;

public record ProductEntryDto(
    string Name,
    decimal Price,
    string Description,
    string Line,
    string Category,
    string Images,
    bool Active
);
