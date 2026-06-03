using DarkKitchen.Importer;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IImporterLoader
{
    IReadOnlyCollection<IProductImporter> LoadImporters();
}
