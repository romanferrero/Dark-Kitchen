using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
{
    public List<Product> GetFiltered(string? line, List<string>? categories, string? name)
    {
        var query = Context.Products.Include(p => p.Images).AsQueryable();

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
