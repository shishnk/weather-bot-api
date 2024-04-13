namespace TelegramBotApp.Messaging.Settings;

public interface ISettings
{
    string HostName { get; }
}

public class RabbitMqSettings : ISettings
{
    public required string HostName { get; init; }
}