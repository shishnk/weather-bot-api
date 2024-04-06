using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Messaging.Connection;
using TelegramBotApp.Messaging.EventBusContext;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.Settings;

namespace TelegramBotApp.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

        services.AddSingleton<IPersistentConnection, PersistentConnection>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<IEventBusSubscriptionsManager, EventBusSubscriptionManager>();
        services.AddSingleton<ISettings, RabbitMqSettings>(_ => settings ?? new RabbitMqSettings
        {
            HostName = "localhost"
        });
        services.AddSingleton<IJsonOptions, JsonOptions>();

        var interfaceType = typeof(IIntegrationEventHandler);
        foreach (var type in
                 AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(s => s.GetTypes())
                     .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)) services.AddTransient(type);

        return services;
    }
}