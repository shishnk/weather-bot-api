using System.Text.Json;
using TelegramBotApp.Messaging.IntegrationContext;

namespace TelegramBotApp.Messaging.Settings;

public interface IJsonOptions
{
    JsonSerializerOptions Options { get; }
}

public class JsonOptions : IJsonOptions
{
    public JsonSerializerOptions Options { get; } = new()
    {
        TypeInfoResolver = new PolymorphicIntegrationEventTypeResolver()
    };
}