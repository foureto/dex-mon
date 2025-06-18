using System.Linq.Expressions;
using Marten;
using Marten.Linq;

namespace dex.monitor.Business.DataStores.Persistant.Internals;

public class BaseMartenRepo<T>(IDocumentSession session) : IRepo<T> where T : class
{
    private IMartenQueryable<T> Q => session.Query<T>();

    public Task Save(CancellationToken ct = default) => session.SaveChangesAsync(ct);

    public Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => Q.AnyAsync(predicate, ct);

    public Task<T> Get(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => Q.FirstOrDefaultAsync(predicate, ct);

    public Task<IReadOnlyList<T>> GetMany(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => Q.Where(predicate).ToListAsync(ct);

    public Task<IReadOnlyList<TK>> GetMany<TK>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TK>> selector,
        CancellationToken ct = default)
        => Q.Where(predicate).Select(selector).ToListAsync(ct);

    public void Store(T entity) => session.Store(entity);

    public void Store(IEnumerable<T> entities) => session.Store(entities);

    public Task StoreAndSave(T entity, CancellationToken ct = default)
    {
        Store(entity);
        return session.SaveChangesAsync(ct);
    }

    public Task StoreAndSave(IEnumerable<T> entities, CancellationToken ct = default)
    {
        Store(entities);
        return session.SaveChangesAsync(ct);
    }
}