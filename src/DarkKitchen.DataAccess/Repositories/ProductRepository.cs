using System.Linq.Expressions;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public List<Product> GetFiltered(Expression<Func<Product, bool>>? predicate = null)
    {
        var query = _context.Products.Include(p => p.Images).AsQueryable();

        if(predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.ToList();
    }
}
