using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public List<Product> GetFiltered(string? line, List<string>? categories, string? name)
    {
        IQueryable<Product> query = _context.Products;

        if(!string.IsNullOrWhiteSpace(line))
        {
            query = query.Where(p => p.Line == line);
        }

        if(categories != null && categories.Count > 0)
        {
            query = query.Where(p => categories.Contains(p.Category));
        }

        if(!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }

        return query.ToList();
    }
}
