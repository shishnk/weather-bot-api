using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging.EventBusContext;

public class EventBusSubscriptionManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers = [];
    private readonly Dictionary<string, List<SubscriptionInfo>> _responseHandlers = [];
    private readonly List<Type> _eventTypes = [];
    private readonly List<Type> _responseTypes = [];

    public void AddSubscription<T, TH>()
        where T : IntegrationEventBase
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        var handlerType = typeof(TH);

        if (!HasSubscriptionsForEvent(eventName)) _handlers.Add(eventName, []);

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
        }

        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));

        if (!_eventTypes.Contains(typeof(T))) _eventTypes.Add(typeof(T));
    }

    public void AddResponseSubscription<T, TH>() where T : IResponseMessage where TH : IResponseHandler<T>
    {
        var replyName = GetEventKey<T>();
        var handlerType = typeof(TH);

        if (!HasSubscriptionsForResponse(replyName)) _responseHandlers.Add(replyName, []);

        if (_responseHandlers[replyName].Any(s => s.HandlerType == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{replyName}'", nameof(handlerType));
        }

        _responseHandlers[replyName].Add(SubscriptionInfo.Typed(handlerType));

        if (!_responseTypes.Contains(typeof(T))) _responseTypes.Add(typeof(T));
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];
    
    public IEnumerable<SubscriptionInfo> GetHandlersForResponse(string replyName) => _responseHandlers[replyName];

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public bool HasSubscriptionsForResponse(string replyName) => _responseHandlers.ContainsKey(replyName);

    public Type? GetEventTypeByName(string eventName) => _eventTypes.Find(t => t.Name == eventName);
    
    public Type? GetResponseTypeByName(string replyName) => _responseTypes.Find(t => t.Name == replyName);

    public string GetEventKey<T>() => typeof(T).Name;
}