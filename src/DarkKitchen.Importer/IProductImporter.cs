namespace DarkKitchen.Importer;

public interface IProductImporter
{
    string Name { get; }

    string Description { get; }

    IReadOnlyCollection<ImporterParameter> Parameters { get; }

    IReadOnlyCollection<ImportedProduct> Import(IReadOnlyDictionary<string, string> arguments);
}
