using RabbitMQ.Client;
using TelegramBotApp.Messaging.EventBusContext;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging;

public interface IMessageFormatter<in T> where T : class
{
    // ReSharper disable once UnusedMemberInSuper.Global
    public string Format(T value);
}

public interface IEventBus
{
    Task<UniversalResponse> Publish(IntegrationEventBase eventBase, string? replyTo = null,
        CancellationToken cancellationToken = default);

    void Subscribe<T, TH>()
        where T : IntegrationEventBase
        where TH : IIntegrationEventHandler<T>;
    
    void SubscribeResponse<T, TH>()
        where T : IResponseMessage
        where TH : IResponseHandler<T>;
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

    void AddResponseSubscription<T, TH>()
        where T : IResponseMessage
        where TH : IResponseHandler<T>;

    bool HasSubscriptionsForEvent(string eventName);
    bool HasSubscriptionsForResponse(string replyName);
    Type? GetEventTypeByName(string eventName);
    Type? GetResponseTypeByName(string replyName);
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    IEnumerable<SubscriptionInfo> GetHandlersForResponse(string replyName);
    string GetEventKey<T>();
}