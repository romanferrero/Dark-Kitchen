using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IProductRepository
{
    List<Product> GetFiltered(string? line, List<string>? categories, string? name);
    void Add(Product product);
    Product? GetByCode(string code);
    void Update(Product product);
}
