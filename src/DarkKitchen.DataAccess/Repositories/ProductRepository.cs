using System.Linq.Expressions;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
{
    public List<Product> GetFiltered(Expression<Func<Product, bool>>? predicate = null)
    {
        var query = Context.Products.Include(p => p.Images).AsQueryable();

        if(predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.ToList();
    }
}
