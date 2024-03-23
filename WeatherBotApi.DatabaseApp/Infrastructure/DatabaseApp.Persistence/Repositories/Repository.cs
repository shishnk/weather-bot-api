using DatabaseApp.Domain.Models;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApp.Persistence.Repositories;

public abstract class RepositoryBase<TEntity>(IDatabaseContext context)
    where TEntity : class, IEntity
{
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly IDatabaseContext _context = context;

    public async Task SaveDbChangesAsync(CancellationToken cancellationToken) =>
        await _context.SaveDbChangesAsync(cancellationToken);

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await _context.SetEntity<TEntity>().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken) =>
        await _context.SetEntity<TEntity>().AddAsync(entity, cancellationToken);

    public void Update(TEntity entity) =>
        _context.SetEntity<TEntity>().Update(entity);

    public void Delete(TEntity entity) =>
        _context.SetEntity<TEntity>().Remove(entity);
}