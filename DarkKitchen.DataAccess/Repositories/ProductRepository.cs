using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public List<Product> GetFiltered(string? line, List<string>? categories, string? name)
    {
        _ = line;
        _ = categories;
        _ = name;
        return _context.Products.ToList();
    }
}
