using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IProductRepository : IRepository<Product>
{
    Product? GetByCode(string code);

    List<Product> GetFiltered(string? line, List<string>? categories, string? name);
}
