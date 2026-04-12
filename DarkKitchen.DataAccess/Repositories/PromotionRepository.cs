using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class PromotionRepository(AppDbContext context) : IPromotionRepository
{
    public void Add(Promotion promotion)
    {
        context.Promotions.Add(promotion);
        context.SaveChanges();
    }

    public Promotion? GetById(int id)
    {
        return context.Promotions
            .Include(p => p.Products)
            .FirstOrDefault(p => p.Id == id);
    }

    public void Update(Promotion promotion)
    {
        context.Promotions.Update(promotion);
        context.SaveChanges();
    }

    public List<Promotion> GetFiltered(DateOnly? date, string? line, string? product)
    {
        var query = context.Promotions.Include(p => p.Products).AsQueryable();

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
