using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;
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
        throw new NotImplementedException();
    }
}
