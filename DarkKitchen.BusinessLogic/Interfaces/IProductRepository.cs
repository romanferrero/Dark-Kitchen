using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IProductRepository
{
    List<Product> GetFiltered(string? line, List<string>? categories, string? name);
}
