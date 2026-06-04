using System.Xml.Serialization;

namespace DarkKitchen.Importer.Xml;

public class XmlProductImporter : IProductImporter
{
    public string Name => "XML";

    public string Description => "Importa productos desde un archivo XML";

    public IReadOnlyCollection<ImporterParameter> Parameters =>
    [
        new ImporterParameter("filePath", "Ruta del archivo", "Ruta absoluta al archivo XML de productos", true)
    ];

    public IReadOnlyCollection<ImportedProduct> Import(IReadOnlyDictionary<string, string> arguments)
    {
        if(!arguments.TryGetValue("filePath", out var filePath) || string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("El parametro 'filePath' es requerido.");
        }

        var serializer = new XmlSerializer(typeof(XmlProductList));
        using var stream = File.OpenRead(filePath);
        var list = (XmlProductList?)serializer.Deserialize(stream)
            ?? throw new InvalidOperationException("El archivo XML no contiene productos validos.");

        return list.Products.Select(ToImportedProduct).ToList();
    }

    private static ImportedProduct ToImportedProduct(XmlProductEntry entry)
    {
        var images = entry.Images
            .Select(img => new ImportedProductImage(img.Path, img.SizeInKb))
            .ToList();

        return new ImportedProduct(entry.Name, entry.Price, entry.Description,
            entry.Line, entry.Category, images, entry.Active);
    }
}

[XmlRoot("Products")]
public class XmlProductList
{
    [XmlElement("Product")]
    public List<XmlProductEntry> Products { get; set; } = [];
}

public class XmlProductEntry
{
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Line { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool Active { get; set; } = true;

    [XmlArray("Images")]
    [XmlArrayItem("Image")]
    public List<XmlImageEntry> Images { get; set; } = [];
}

public class XmlImageEntry
{
    public string Path { get; set; } = string.Empty;

    public decimal SizeInKb { get; set; }
}
