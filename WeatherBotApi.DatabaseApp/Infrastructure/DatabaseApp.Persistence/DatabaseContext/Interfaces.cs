using DatabaseApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DatabaseApp.Persistence.DatabaseContext;

public interface IDatabaseContext
{
    DbSet<UserWeatherSubscription> UserWeatherSubscriptions { get; }
    DbSet<User> Users { get; }
    DatabaseFacade Db { get; }

    public Task SaveDbChangesAsync(CancellationToken cancellationToken);
    public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IEntity;
    public void DisposeResources();
}