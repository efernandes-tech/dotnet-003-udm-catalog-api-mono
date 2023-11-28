using Api.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Api.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected CatalogoApiDbContext _context;

    public Repository(CatalogoApiDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Get()
    {
        return _context.Set<T>().AsNoTracking();
    }

    public async Task<T> GetByIdAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().SingleOrDefaultAsync(predicate);
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
