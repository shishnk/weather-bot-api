using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TelegramBotApp.Domain.Models;

namespace TelegramBotApp.Messaging.RabbitMqProducer;

public class MessageProducer : IMessageProducer
{
    private const string QueueName = "weather-forecast";

    private bool _disposed;
    private IModel? _channel;
    private IConnection? _connection;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<IResponseMessage>> _callbackMapper = new();

    public bool IsInitialized { get; private set; }

    ~MessageProducer() => Dispose(false);

    public void EnsureInitialize()
    {
        IsInitialized = true;

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        var consumer = new EventingBasicConsumer(_channel);

        _channel.ExchangeDeclare(exchange: "weather-forecast", type: ExchangeType.Direct);
        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _channel.QueueBind(
            queue: QueueName,
            exchange: "weather-forecast",
            routingKey: QueueName);

        consumer.Received += (_, ea) =>
        {
            if (!_callbackMapper.TryRemove(ea.BasicProperties.CorrelationId ??
                                           throw new ArgumentNullException(nameof(ea.BasicProperties.CorrelationId)),
                    out var tcs)) return;

            // var response = JsonSerializer.Deserialize<>(body) ??
                           // throw new InvalidOperationException("Deserialization failed");
            tcs.TrySetResult(new Kek());
        };

        _channel.BasicConsume(queue: "weather-forecast", autoAck: true, consumer: consumer);
    }

    public Task<IResponseMessage> PublishMessageAsync<TRequest>(TRequest message, CancellationToken cancellationToken)
    {
        if (_channel == null) throw new InvalidOperationException("Channel is not initialized/registered");

        var properties = _channel.CreateBasicProperties();
        properties.CorrelationId = Guid.NewGuid().ToString();
        properties.ReplyTo = QueueName;

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var tcs = new TaskCompletionSource<IResponseMessage>();
        _callbackMapper.TryAdd(properties.CorrelationId, tcs);

        _channel.BasicPublish(
            exchange: "weather-forecast",
            routingKey: QueueName,
            basicProperties: properties,
            body: body);

        cancellationToken.Register(() => _callbackMapper.TryRemove(properties.CorrelationId, out _));
        return tcs.Task;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // Dispose managed resources
        }

        _channel?.Dispose();
        _connection?.Dispose();

        _disposed = true;
    }
}