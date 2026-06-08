namespace DarkKitchen.Domain.Entities;

public record CreateProductParamsDto(
    string Code,
    string Name,
    decimal Price,
    string Description,
    string Line,
    string Category,
    string Images,
    bool Active);
