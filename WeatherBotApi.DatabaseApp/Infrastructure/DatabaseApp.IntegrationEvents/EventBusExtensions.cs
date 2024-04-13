using DatabaseApp.IntegrationEvents.IntegrationEventHandlers;
using DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserIntegrationEventHandlers;
using DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserSubscriptionEventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;

namespace DatabaseApp.IntegrationEvents;

public static class EventBusExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IApplicationBuilder SubscribeToEvents(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        eventBus.Subscribe<CacheRequestUsersIntegrationEvent, CacheRequestUsersIntegrationEventHandler>();
        eventBus.Subscribe<CacheRequestUserSubscriptionsIntegrationEvent, CacheRequestUserSubscriptionsIntegrationEventHandler>();
        eventBus.Subscribe<CreatedUserIntegrationEvent, CreatedUserIntegrationEventHandler>();
        eventBus.Subscribe<CreatedUserSubscriptionIntegrationEvent, CreatedUserSubscriptionIntegrationEventHandler>();
        eventBus.Subscribe<UpdatedUserSubscriptionIntegrationEvent, UpdatedUserSubscriptionIntegrationEventHandler>();
        eventBus.Subscribe<DeletedUserSubscriptionIntegrationEvent, DeletedUserSubscriptionIntegrationEventHandler>();

        return app;
    }
}