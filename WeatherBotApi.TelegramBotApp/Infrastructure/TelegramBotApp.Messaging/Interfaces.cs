using RabbitMQ.Client;
using TelegramBotApp.Domain.Responses;
using TelegramBotApp.Messaging.EventBusContext;
using TelegramBotApp.Messaging.IntegrationContext;

namespace TelegramBotApp.Messaging;

public interface IMessageFormatter<in T> where T : class
{
    // ReSharper disable once UnusedMemberInSuper.Global
    public string Format(T value);
}

public interface IEventBus
{
    Task<UniversalResponse?> Publish(IntegrationEventBase eventBase, string? replyTo = null,
        CancellationToken cancellationToken = default);

    void Subscribe<T, TH>()
        where T : IntegrationEventBase
        where TH : IIntegrationEventHandler<T>;
    
    // TODO: create classes with generic parameter (where parameter is response type) 
    void SubscribeToResponse(string replyName);
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
        where T : IntegrationEventBase
        where TH : IIntegrationEventHandler<T>;

    bool HasSubscriptionsForEvent(string eventName);
    Type? GetEventTypeByName(string eventName);
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}