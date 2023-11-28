using System.Linq.Expressions;

namespace Api.Repositories;

public interface IRepository<T>
{
    IQueryable<T> Get();

    Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}
