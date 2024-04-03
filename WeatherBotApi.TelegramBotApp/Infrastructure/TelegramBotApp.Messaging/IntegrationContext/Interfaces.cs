using TelegramBotApp.Domain.Models;

namespace TelegramBotApp.Messaging.IntegrationContext;

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task<IResponseMessage?> Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler;