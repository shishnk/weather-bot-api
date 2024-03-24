using DatabaseApp.Domain.Models;
using DatabaseApp.Persistence.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DatabaseApp.Persistence.DatabaseContext;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IDatabaseContext
{
    public required DbSet<UserWeatherSubscription> UserWeatherSubscriptions { get; init; }
    public required DbSet<WeatherDescription> WeatherDescriptions { get; init; }
    public required DbSet<User> Users { get; init; }
    public DatabaseFacade Db => Database;

    public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IEntity => Set<TEntity>();

    public Task SaveDbChangesAsync(CancellationToken cancellationToken) => SaveChangesAsync(cancellationToken);

    public void DisposeResources() => Dispose();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WeatherDescriptionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserWeatherSubscriptionConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}