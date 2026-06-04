using System.Text.Json;

namespace DarkKitchen.Importer.Json;

public class JsonProductImporter : IProductImporter
{
    public string Name => "JSON";

    public string Description => "Importa productos desde un archivo JSON";

    public IReadOnlyCollection<ImporterParameter> Parameters =>
    [
        new ImporterParameter("filePath", "Ruta del archivo", "Ruta absoluta al archivo JSON de productos", true)
    ];

    public IReadOnlyCollection<ImportedProduct> Import(IReadOnlyDictionary<string, string> arguments)
    {
        if(!arguments.TryGetValue("filePath", out var filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("El parametro 'filePath' es requerido.");
        }

        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var items = JsonSerializer.Deserialize<List<JsonProductEntry>>(json, options)
            ?? throw new InvalidOperationException("El archivo JSON no contiene productos validos.");

        return items.Select(ToImportedProduct).ToList();
    }

    private static ImportedProduct ToImportedProduct(JsonProductEntry entry)
    {
        var images = entry.Images
            .Select(img => new ImportedProductImage(img.Path, img.SizeInKb))
            .ToList();

        return new ImportedProduct(entry.Name, entry.Price, entry.Description,
            entry.Line, entry.Category, images, entry.Active);
    }
}

internal sealed class JsonProductEntry
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Line { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    public List<JsonImageEntry> Images { get; set; } = [];
}

internal sealed class JsonImageEntry
{
    public string Path { get; set; } = string.Empty;

    public decimal SizeInKb { get; set; }
}
