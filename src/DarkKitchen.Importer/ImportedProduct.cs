namespace DarkKitchen.Importer;

public record ImportedProduct(
    string Name,
    decimal Price,
    string Description,
    string Line,
    string Category,
    IReadOnlyCollection<ImportedProductImage> Images,
    bool Active);
