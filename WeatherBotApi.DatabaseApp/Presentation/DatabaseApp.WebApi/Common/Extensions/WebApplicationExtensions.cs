using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DatabaseApp.WebApi.Common.Extensions;

public static class WebApplicationExtensions
{
    public static void MigrateDatabase(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            databaseContext.Db.Migrate();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "An error occurred while migrating the database.");
        }
    }
}