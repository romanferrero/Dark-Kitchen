using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository : IRepository<Product>
{
    List<Product> GetFiltered(string? line, List<string>? categories, string? name);
}
