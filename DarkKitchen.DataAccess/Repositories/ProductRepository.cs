using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public List<Product> GetFiltered(string? line, List<string>? categories, string? name)
    {
        _ = context.Products.AsQueryable();

        throw new NotImplementedException();
    }
}
