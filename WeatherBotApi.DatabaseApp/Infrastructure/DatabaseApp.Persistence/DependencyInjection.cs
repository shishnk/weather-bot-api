using DatabaseApp.Domain.Repositories;
using DatabaseApp.Persistence.DatabaseContext;
using DatabaseApp.Persistence.Repositories;
using DatabaseApp.Persistence.UnitOfWorkContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseApp.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<DatabaseContext.DatabaseContext>(options =>
            options.UseNpgsql(connectionString, builder =>
                builder.MigrationsAssembly(typeof(DatabaseContext.DatabaseContext).Assembly.FullName)));
        services.AddScoped<IDatabaseContext, DatabaseContext.DatabaseContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWeatherDescriptionRepository, WeatherDescriptionRepository>();
        services.AddScoped<IWeatherSubscriptionRepository, WeatherSubscriptionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}