using System.Linq.Expressions;
using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class PromotionRepository(AppDbContext context) : Repository<Promotion>(context), IPromotionRepository
{
    public override List<Promotion> GetAll(Expression<Func<Promotion, bool>>? predicate = null)
    {
        var query = Context.Promotions.Include(p => p.Products).AsQueryable();

        if(predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.ToList();
    }

    public List<Promotion> GetFiltered(DateOnly? date, string? line, string? product)
    {
        var query = Context.Promotions.Include(p => p.Products).AsQueryable();

        if(date.HasValue)
        {
            query = query.Where(p => p.DateFrom <= date.Value && p.DateTo >= date.Value);
        }

        if(!string.IsNullOrEmpty(line))
        {
            query = query.Where(p => p.Products.Any(pr => pr.Line == line));
        }

        if(!string.IsNullOrEmpty(product))
        {
            query = query.Where(p => p.Products.Any(pr => pr.Code == product || pr.Name.ToLower().Contains(product.ToLower())));
        }

        return query.ToList();
    }
}
