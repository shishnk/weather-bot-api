using DatabaseApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DatabaseApp.Persistence.Initialization;

public interface IDatabaseContext
{
    DbSet<UserWeatherSubscription> WeatherSubscriptions { get; init; }
    DbSet<WeatherDescription> WeatherDescriptions { get; init; }
    DbSet<User> Users { get; init; }
    DatabaseFacade Db { get; }
}