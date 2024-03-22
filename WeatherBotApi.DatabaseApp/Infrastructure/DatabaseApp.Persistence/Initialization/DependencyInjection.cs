using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseApp.Persistence.Initialization;

public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");

        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(connectionString, builder => builder.MigrationsAssembly("DatabaseApp.Persistence")));
        services.AddScoped<IDatabaseContext, DatabaseContext>();
    }
}