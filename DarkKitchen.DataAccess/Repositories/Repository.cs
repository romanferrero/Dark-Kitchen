using System.Linq.Expressions;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T>
    where T : class
{
    protected AppDbContext Context { get; } = context;

    public void Add(T entity)
    {
        Context.Set<T>().Add(entity);
        Context.SaveChanges();
    }

    public void Update(T entity)
    {
        Context.Set<T>().Update(entity);
        Context.SaveChanges();
    }

    public virtual List<T> GetAll(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            return Context.Set<T>().ToList();
        }

        return Context.Set<T>().Where(predicate).ToList();
    }
}
