using TelegramBotApp.Messaging.IntegrationContext;

namespace TelegramBotApp.Messaging.EventBusContext;

public class EventBusSubscriptionManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers = [];
    private readonly List<Type> _eventTypes = [];

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

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public Type? GetEventTypeByName(string eventName) => _eventTypes.Find(t => t.Name == eventName);
    
    public string GetEventKey<T>() => typeof(T).Name;
}