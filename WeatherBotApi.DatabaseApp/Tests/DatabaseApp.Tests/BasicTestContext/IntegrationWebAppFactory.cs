using System.Data.Common;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.Settings;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace DatabaseApp.Tests.BasicTestContext;

// ReSharper disable once ClassNeverInstantiated.Global
public class IntegrationWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("weather-database")
        .WithUsername("user")
        .WithPassword("pass").Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:latest")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    private Respawner _respawner = null!;
    private DbConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<IDatabaseContext, ApplicationDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            descriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IMessageSettings));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IMessageSettings>(new RabbitMqSettings
            {
                EventExchangeName = "event-exchange",
                ResponseExchangeName = "response-exchange",
                EventQueueName = "event-queue",
                ResponseQueueName = "response-queue",
                ConnectionString = _rabbitMqContainer.GetConnectionString()
            });
        });
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_connection);

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        _connection = Services.CreateScope().ServiceProvider.GetRequiredService<IDatabaseContext>()
            .Db.GetDbConnection();
        await _connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_connection, new()
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }

    public new async Task DisposeAsync() =>
        await Task.WhenAll(
            _dbContainer.DisposeAsync().AsTask(),
            _rabbitMqContainer.DisposeAsync().AsTask());
}