using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IPromotionRepository : IRepository<Promotion>
{
    List<Promotion> GetFiltered(DateOnly? date, string? line, string? product);
}
