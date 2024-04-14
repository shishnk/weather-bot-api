namespace TelegramBotApp.Messaging.Settings;

public interface IMessageSettings
{
    string? HostName { get; }
    int Port { get; }
    string? Username { get; }
    string? Password { get; }
    string EventQueueName { get; }
    string ResponseQueueName { get; }
    string EventExchangeName { get; }
    string ResponseExchangeName { get; }
    string? ConnectionString { get; }
    bool HasConnectionString { get; }
}

public class RabbitMqSettings : IMessageSettings
{
    public string? HostName { get; init; }
    public int Port { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public required string EventQueueName { get; init; }
    public required string ResponseQueueName { get; init; }
    public required string EventExchangeName { get; init; }
    public required string ResponseExchangeName { get; init; }
    public string? ConnectionString { get; init; }
    public bool HasConnectionString => !string.IsNullOrWhiteSpace(ConnectionString);
}