using DatabaseApp.Domain.Models;
using DatabaseApp.Persistence.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DatabaseApp.Persistence.Initialization;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options), IDatabaseContext
{
    public required DbSet<UserWeatherSubscription> WeatherSubscriptions { get; init; }
    public required DbSet<WeatherDescription> WeatherDescriptions { get; init; }
    public required DbSet<User> Users { get; init; }
    public DatabaseFacade Db => Database;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WeatherDescriptionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserWeatherSubscriptionConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}