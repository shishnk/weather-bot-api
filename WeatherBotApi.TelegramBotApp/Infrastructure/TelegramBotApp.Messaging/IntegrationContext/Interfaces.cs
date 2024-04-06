using TelegramBotApp.Domain.Responses;

namespace TelegramBotApp.Messaging.IntegrationContext;

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEventBase
{
    Task<UniversalResponse?> Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler;