using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

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
        throw new NotImplementedException();
    }

    public void Update(Promotion promotion)
    {
        throw new NotImplementedException();
    }

    public List<Promotion> GetFiltered(DateOnly? date, string? line, string? product)
    {
        throw new NotImplementedException();
    }
}
