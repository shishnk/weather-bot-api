using TelegramBotApp.Domain.Models;

namespace TelegramBotApp.Messaging.RabbitMqProducer;

public interface IMessageProducer : IDisposable
{
    bool IsInitialized { get; }

    void EnsureInitialize(); // in 7.0 version RabbitMQ has async methods (but not all features are supported)
    Task<IResponseMessage> PublishMessageAsync<TRequest>(TRequest message, CancellationToken cancellationToken);
}