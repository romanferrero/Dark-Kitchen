using System.Linq.Expressions;

namespace DarkKitchen.IDataAccess;

public interface IRepository<T>
    where T : class
{
    void Add(T entity);

    void Update(T entity);

    void Delete(Expression<Func<T, bool>> predicate);

    List<T> GetAll(Expression<Func<T, bool>>? predicate = null);
}
