using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public void Add(Product product) => throw new NotImplementedException();

    public List<Product> GetFiltered(string? line, List<string>? categories, string? name)
    {
        var query = context.Products.AsQueryable();

        if(!string.IsNullOrEmpty(line))
        {
            query = query.Where(p => p.Line == line);
        }

        if(categories is not null && categories.Count > 0)
        {
            query = query.Where(p => categories.Contains(p.Category));
        }

        if(!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
        }

        return query.ToList();
    }
}
