using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IProductRepository : IRepository<Product>
{
    List<Product> GetFiltered(Expression<Func<Product, bool>>? predicate = null);
}
