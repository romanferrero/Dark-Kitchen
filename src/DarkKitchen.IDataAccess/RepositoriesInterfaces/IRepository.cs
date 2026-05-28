using System.Linq.Expressions;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IRepository<T>
    where T : class
{
    void Add(T entity);

    void Update(T entity);

    void Delete(Expression<Func<T, bool>> predicate);

    List<T> GetAll(Expression<Func<T, bool>>? predicate = null);

    T? Get(Expression<Func<T, bool>> predicate);

    bool Exists(Expression<Func<T, bool>> predicate);
}
