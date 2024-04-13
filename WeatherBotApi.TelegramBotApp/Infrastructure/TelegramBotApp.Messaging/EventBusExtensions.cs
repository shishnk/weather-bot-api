using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging;

public static class EventBusExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IHost SubscribeToResponses(this IHost app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();

        eventBus.SubscribeResponse<UniversalResponse, UniversalResponseHandler>();

        return app;
    }
}