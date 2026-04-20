using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IProductRepository : IRepository<Product>
{
    List<Product> GetFiltered(string? line, List<string>? categories, string? name);
}
