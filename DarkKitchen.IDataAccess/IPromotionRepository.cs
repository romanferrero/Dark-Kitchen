using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IPromotionRepository
{
    void Add(Promotion promotion);

    Promotion? GetById(int id);

    void Update(Promotion promotion);

    List<Promotion> GetFiltered(DateOnly? date, string? line, string? product);
}
