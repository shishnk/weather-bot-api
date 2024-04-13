using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TelegramBotApp.Messaging;

public static class ReplyNames
{
    public const string UniversalReply = "Universal";
}

public static class EventBusExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IHost SubscribeToResponses(this IHost app)
    {
        var eventBus = app.Services.GetRequiredService<IEventBus>();
        
        // TODO: subscribe to response with handler
        eventBus.SubscribeToResponse(ReplyNames.UniversalReply);

        return app;
    }
}