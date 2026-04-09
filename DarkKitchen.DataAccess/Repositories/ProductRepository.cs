using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public List<Product> GetFiltered(string? line, List<string>? categories, string? name)
    {
        IQueryable<Product> query = _context.Products;
        _ = categories;
        _ = name;

        if(line != null)
        {
            query = query.Where(p => p.Line == line);
        }

        return query.ToList();
    }
}
