using System.Linq.Expressions;

namespace dex.monitor.Business.DataStores.Persistant;

public interface IRepo<T> where T : class
{
    Task Save(CancellationToken ct = default);
    Task<bool> Any(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<T> Get(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<IReadOnlyList<T>> GetMany(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<IReadOnlyList<TK>> GetMany<TK>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TK>> selector,
        CancellationToken ct = default);

    void Store(T entity);
    void Store(IEnumerable<T> entities);

    Task StoreAndSave(T entity, CancellationToken ct = default);
    Task StoreAndSave(IEnumerable<T> entities, CancellationToken ct = default);
}