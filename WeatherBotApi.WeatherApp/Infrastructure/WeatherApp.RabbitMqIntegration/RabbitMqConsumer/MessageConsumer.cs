using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TelegramBotApp.Domain.Models;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Models;

namespace WeatherApp.RabbitMqIntegration.RabbitMqConsumer;

public sealed class MessageConsumer : BackgroundService
{
    private const string QueueName = "weather-forecast";

    private bool _disposed;
    private readonly ILogger<MessageConsumer> _logger;
    private IModel? _channel;
    private IConnection? _connection;
    private readonly IWeatherService _weatherService;

    ~MessageConsumer() => Dispose(false);

    public MessageConsumer(IWeatherService weatherService, ILogger<MessageConsumer> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
        InitializeRabbitMq();
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("RabbitMQ consumer started and listening for messages");

        // with AsyncEventingBasicConsumer there is problem (not receiving messages)
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            _logger.LogInformation("Received message from RabbitMQ");

            WeatherDescriptor? weatherDescriptor = null;
            var requestProperties = ea.BasicProperties;
            var responseProperties = _channel!.CreateBasicProperties();
            responseProperties.CorrelationId = requestProperties.CorrelationId;

            try
            {
                var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(ea.Body.ToArray());
                weatherDescriptor =
                    await _weatherService.GetWeatherForecastAsync(weatherRequest?.Location ?? string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while processing the message");
            }
            finally
            {
                _logger.LogInformation("Sending response to RabbitMQ");
                _channel.BasicPublish(exchange: "weather-forecast",
                    routingKey: ea.RoutingKey,
                    basicProperties: responseProperties,
                    body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(weatherDescriptor ?? null))); // TODO: 
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // dispose managed resources
        }

        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();

        _disposed = true;
    }

    private void InitializeRabbitMq()
    {
        _logger.LogInformation("Starting RabbitMQ consumer");

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: "weather-forecast", type: ExchangeType.Direct);
        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _channel.QueueBind(queue: QueueName, exchange: "weather-forecast", routingKey: "weather-forecast");
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }
}