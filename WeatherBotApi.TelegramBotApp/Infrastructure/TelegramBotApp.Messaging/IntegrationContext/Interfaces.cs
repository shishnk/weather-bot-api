using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging.IntegrationContext;

public interface IIntegrationEventHandler;

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEventBase
{
    Task<IResponseMessage?> Handle(TIntegrationEvent @event);
}