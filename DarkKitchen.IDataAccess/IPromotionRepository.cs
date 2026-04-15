using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IPromotionRepository : IRepository<Promotion>
{
    List<Promotion> GetFiltered(DateOnly? date, string? line, string? product);
}
