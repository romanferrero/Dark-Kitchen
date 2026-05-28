using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IPromotionRepository : IRepository<Promotion>
{
    List<Promotion> GetFiltered(Expression<Func<Promotion, bool>>? predicate = null);
}
