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
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, builder =>
                builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        services.AddScoped<IDatabaseContext, ApplicationDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWeatherSubscriptionRepository, WeatherSubscriptionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddHealthChecks()
            .AddNpgSql(connectionString ?? throw new InvalidOperationException("DbConnection string is null"));

        return services;
    }
}