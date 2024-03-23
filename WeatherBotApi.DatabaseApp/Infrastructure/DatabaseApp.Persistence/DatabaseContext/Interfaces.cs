using DatabaseApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DatabaseApp.Persistence.DatabaseContext;

public interface IDatabaseContext
{
    DbSet<UserWeatherSubscription> UserWeatherSubscriptions { get; init; }
    DbSet<WeatherDescription> WeatherDescriptions { get; init; }
    DbSet<User> Users { get; init; }
    DatabaseFacade Db { get; }

    public Task SaveDbChangesAsync(CancellationToken cancellationToken);
    public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IEntity;
    public void DisposeResources();
}