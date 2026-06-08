using System.Reflection;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.Importer;

namespace DarkKitchen.BusinessLogic.Importing;

public sealed class ReflectionImporterLoader(string pluginsPath) : IImporterLoader
{
    public IReadOnlyCollection<IProductImporter> LoadImporters()
    {
        if(!Directory.Exists(pluginsPath))
        {
            return [];
        }

        var importers = new List<IProductImporter>();

        foreach(var file in Directory.GetFiles(pluginsPath, "*.dll"))
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(file));
                var importerTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IProductImporter).IsAssignableFrom(t));

                foreach(var type in importerTypes)
                {
                    if(Activator.CreateInstance(type) is IProductImporter importer)
                    {
                        importers.Add(importer);
                    }
                }
            }
            catch(Exception)
            {
                // DLL invalida o incompatible, se ignora y se continua con las demas
            }
        }

        return importers;
    }
}
