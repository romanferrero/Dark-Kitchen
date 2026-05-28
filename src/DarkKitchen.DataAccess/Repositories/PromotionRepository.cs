using System.Linq.Expressions;
using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
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

    public override Promotion? Get(Expression<Func<Promotion, bool>> predicate)
    {
        return Context.Promotions.Include(p => p.Products).FirstOrDefault(predicate);
    }

    public List<Promotion> GetFiltered(Expression<Func<Promotion, bool>>? predicate = null)
    {
        var query = Context.Promotions.Include(p => p.Products).AsQueryable();

        if(predicate != null)
        {
            query = query.Where(predicate);
        }

        return query.ToList();
    }
}
