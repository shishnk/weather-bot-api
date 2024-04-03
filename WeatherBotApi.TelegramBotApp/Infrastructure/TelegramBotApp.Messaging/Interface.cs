using RabbitMQ.Client;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging.EventBusContext;
using TelegramBotApp.Messaging.IntegrationContext;

namespace TelegramBotApp.Messaging;

public interface IMessageProducer : IDisposable
{
    bool IsInitialized { get; }

    void EnsureInitialize(); // in 7.0 version RabbitMQ has async methods (but not all features are supported)
    Task<IResponseMessage> PublishMessageAsync<TRequest>(TRequest message, CancellationToken cancellationToken);
}

public interface IEventBus
{
    bool IsInitialized { get; }

    void EnsureInitialize();

    Task<IResponseMessage?> Publish(IntegrationEvent @event, string? replyTo = null,
        CancellationToken cancellationToken = default);

    void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    void SubscribeOnResponse<T>() where T : IResponseMessage;
}

public interface IPersistentConnection : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();
    IModel CreateModel();
}

public interface IEventBusSubscriptionsManager
{
    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    bool HasSubscriptionsForEvent(string eventName);
    Type? GetEventTypeByName(string eventName);
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}