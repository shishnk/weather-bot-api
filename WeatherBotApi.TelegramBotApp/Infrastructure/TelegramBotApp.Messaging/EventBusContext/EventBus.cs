using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;
using TelegramBotApp.Messaging.Settings;

namespace TelegramBotApp.Messaging.EventBusContext;

public class EventBus(
    IPersistentConnection persistentConnection,
    ILogger<EventBus> logger,
    IEventBusSubscriptionsManager subscriptionsManager,
    IServiceProvider serviceProvider,
    IJsonOptions jsonOptions,
    int retryCount = 3)
    : IEventBus
{
    private const string DirectEventExchange = "direct-event-exchange";
    private const string DirectResponseExchange = "direct-response-exchange";
    private const string EventQueue = "event-queue";
    private const string ResponseQueue = "response-queue";

    private IModel _channel = null!;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<UniversalResponse>> _callbackMapper = new();
    private bool _isInitialized;

    public Task<UniversalResponse> Publish(IntegrationEventBase eventBase, string? replyTo = null,
        CancellationToken cancellationToken = default)
    {
        if (!_isInitialized) EnsureInitialize();

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) => logger.LogWarning(ex,
                    "Could not publish event: {EventName} after {Timeout}s ({ExceptionMessage})",
                    eventBase.Name, $"{time.TotalSeconds:n1}", ex.Message));

        var body = JsonSerializer.SerializeToUtf8Bytes(eventBase, jsonOptions.Options);
        var tcs = new TaskCompletionSource<UniversalResponse>();
        var properties = _channel.CreateBasicProperties();
        properties.CorrelationId = eventBase.Id.ToString();
        properties.ReplyTo = replyTo;
        _callbackMapper.TryAdd(properties.CorrelationId, tcs);

        cancellationToken.Register(() =>
        {
            _callbackMapper.TryRemove(properties.CorrelationId, out _);
            tcs.TrySetResult(new("Bad request and was canceled"));
        });

        policy.Execute(() =>
        {
            logger.LogInformation("Publishing event to RabbitMQ: {EventName}", eventBase.Name);
            _channel.BasicPublish(
                exchange: DirectEventExchange,
                routingKey: eventBase.Name,
                basicProperties: properties,
                body: body);
        });

        return tcs.Task;
    }

    public void Subscribe<T, TH>()
        where T : IntegrationEventBase
        where TH : IIntegrationEventHandler<T>
    {
        if (!_isInitialized) EnsureInitialize();

        var eventName = subscriptionsManager.GetEventKey<T>();

        if (subscriptionsManager.HasSubscriptionsForEvent(eventName)) return;

        _channel.QueueBind(queue: EventQueue,
            exchange: DirectEventExchange,
            routingKey: eventName);

        logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName,
            typeof(TH).Name);

        subscriptionsManager.AddSubscription<T, TH>();

        StartConsume();
    }

    public void SubscribeResponse<T, TH>()
        where T : IResponseMessage
        where TH : IResponseHandler<T>
    {
        if (!_isInitialized) EnsureInitialize();

        var replyName = subscriptionsManager.GetEventKey<T>();

        if (subscriptionsManager.HasSubscriptionsForResponse(replyName)) return;

        _channel.QueueBind(queue: ResponseQueue,
            exchange: DirectResponseExchange,
            routingKey: replyName);

        logger.LogInformation("Subscribing to response {ResponseName}", replyName);

        subscriptionsManager.AddResponseSubscription<T, TH>();

        var responseConsumer = new AsyncEventingBasicConsumer(_channel);
        
        // TODO: avoid code duplication
        responseConsumer.Received += async (_, ea) =>
        {
            if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId,
                    out var tcs))
            {
                logger.LogWarning("Could not find callback for correlation id: {CorrelationId}",
                    ea.BasicProperties.CorrelationId);
                return;
            }

            var reply = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body.Span);

            logger.LogInformation("Processing RabbitMQ event: {ResponseName}", reply);

            if (subscriptionsManager.HasSubscriptionsForResponse(reply))
            {
                foreach (var subscription in subscriptionsManager.GetHandlersForResponse(reply))
                {
                    var handler = serviceProvider.GetService(subscription.HandlerType);
                    var responseType = subscriptionsManager.GetResponseTypeByName(reply);

                    if (handler == null || responseType == null)
                    {
                        logger.LogWarning(
                            "Could not resolve response handler or response type for RabbitMQ event: {ResponseName}",
                            reply);
                        continue;
                    }

                    var integrationResponse = JsonSerializer.Deserialize(message, responseType, jsonOptions.Options);
                    var concreteType = typeof(IResponseHandler<>).MakeGenericType(responseType);

                    var task = concreteType.GetMethod("Handle")
                                   ?.Invoke(handler, [integrationResponse]) as Task<UniversalResponse> ??
                               throw new InvalidCastException();
                    var response = await task;

                    tcs.TrySetResult(response);
                }
            }
            else
            {
                logger.LogWarning("No subscription for RabbitMQ response: {ResponseName}", reply);
            }
        };

        _channel.BasicConsume(queue: ResponseQueue, autoAck: true, consumer: responseConsumer);
    }

    private void EnsureInitialize()
    {
        if (!persistentConnection.IsConnected)
        {
            if (!persistentConnection.TryConnect())
            {
                logger.LogError("Could not connect to RabbitMQ");
                return;
            }
        }

        _channel = persistentConnection.CreateModel();

        _channel.ExchangeDeclare(exchange: DirectEventExchange, type: ExchangeType.Direct);
        _channel.ExchangeDeclare(exchange: DirectResponseExchange, type: ExchangeType.Direct);

        _channel.QueueDeclare(
            queue: EventQueue,
            durable: true,
            exclusive: false,
            autoDelete: true,
            arguments: null);
        _channel.QueueDeclare(
            queue: ResponseQueue,
            durable: true,
            exclusive: false,
            autoDelete: true,
            arguments: null);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _isInitialized = true;
    }

    private void StartConsume()
    {
        var eventConsumer = new AsyncEventingBasicConsumer(_channel);

        eventConsumer.Received += async (_, ea) =>
        {
            var eventName = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body.Span);

            logger.LogInformation("Processing RabbitMQ event: {EventName}", eventName);

            if (subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                foreach (var subscription in subscriptionsManager.GetHandlersForEvent(eventName))
                {
                    var handler = serviceProvider.GetService(subscription.HandlerType);
                    var eventType = subscriptionsManager.GetEventTypeByName(eventName);

                    if (handler == null || eventType == null)
                    {
                        logger.LogWarning("Could not resolve handler or event type for RabbitMQ event: {EventName}",
                            eventName);
                        continue;
                    }

                    var integrationEvent =
                        JsonSerializer.Deserialize<IntegrationEventBase>(message, jsonOptions.Options);
                    integrationEvent?.UpdateId(Guid.Parse(ea.BasicProperties.CorrelationId));
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    var task = concreteType.GetMethod("Handle")
                                   ?.Invoke(handler, [integrationEvent]) as Task<IResponseMessage?> ??
                               throw new InvalidCastException();
                    var response = await task;

                    if (response == null) continue;

                    var properties = _channel.CreateBasicProperties();
                    properties.CorrelationId = ea.BasicProperties.CorrelationId;

                    _channel.BasicPublish(
                        exchange: DirectResponseExchange,
                        routingKey: ea.BasicProperties.ReplyTo,
                        basicProperties: properties,
                        body: JsonSerializer.SerializeToUtf8Bytes(response, jsonOptions.Options));
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
            }
            else
            {
                logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        };

        _channel.BasicConsume(queue: EventQueue, autoAck: false, consumer: eventConsumer);
    }
}