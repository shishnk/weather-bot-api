using DatabaseApp.IntegrationEvents.IntegrationEventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

namespace DatabaseApp.IntegrationEvents;

public static class EventBusExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IApplicationBuilder SubscribeToEvents(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<GetAllUsersRequestIntegrationEvent, GetAllUsersRequestIntegrationEventHandler>();

        return app;
    }
}