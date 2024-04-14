using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using TelegramBotApp.Messaging.Settings;

namespace TelegramBotApp.Messaging.Connection;

public sealed class PersistentConnection(
    IMessageSettings messageSettings,
    ILogger<PersistentConnection> logger,
    int retryCount = 3)
    : IPersistentConnection
{
    private IConnection? _connection;
    private bool _disposed;

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    ~PersistentConnection() => Dispose(false);

    public bool TryConnect()
    {
        logger.LogInformation("RabbitMQ Client is trying to connect...");

        var policy = Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) => logger.LogWarning(ex,
                    "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                    $"{time.TotalSeconds:n1}", ex.Message));
        
        ConnectionFactory connectionFactory;
        
        if (messageSettings.HasConnectionString)
        {
            connectionFactory = new()
            {
                Uri = new(messageSettings.ConnectionString!),
                DispatchConsumersAsync = true
            }; 
        }
        else
        {
            connectionFactory = new()
            {
                HostName = messageSettings.HostName,
                Port = messageSettings.Port,
                DispatchConsumersAsync = true
            };
        }

        policy.Execute(() => _connection = connectionFactory.CreateConnection());

        if (_connection == null)
        {
            throw new InvalidOperationException("RabbitMQ connection could not be created and opened");
        }

        if (IsConnected)
        {
            _connection.ConnectionShutdown += (_, _) =>
            {
                if (_disposed) return;
                logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
                TryConnect();
            };
            _connection.CallbackException += (_, _) =>
            {
                if (_disposed) return;
                logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
                TryConnect();
            };
            _connection.ConnectionBlocked += (_, _) =>
            {
                if (_disposed) return;
                logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
                TryConnect();
            };

            logger.LogInformation(
                "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events",
                _connection.Endpoint.HostName);

            return true;
        }

        logger.LogCritical("RabbitMQ connections could not be created and opened");

        return false;
    }

    public IModel CreateModel() => _connection?.CreateModel() ??
                                   throw new InvalidOperationException("No RabbitMQ connections are available");

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

        _connection?.Dispose();
        _disposed = true;
    }
}