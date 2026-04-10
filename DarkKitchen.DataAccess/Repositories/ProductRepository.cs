using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public void Add(Product product)
    {
        context.Products.Add(product);
        context.SaveChanges();
    }

    public Product? GetByCode(string code)
    {
        return context.Products
            .Include(p => p.Images)
            .FirstOrDefault(p => p.Code == code);
    }

    public void Update(Product product)
    {
        context.Products.Update(product);
        context.SaveChanges();
    }

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
