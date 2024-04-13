using System.Text.Json;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging.Settings;

public interface IJsonOptions
{
    JsonSerializerOptions Options { get; }
}

public class JsonOptions : IJsonOptions
{
    public JsonSerializerOptions Options { get; } = new()
    {
        TypeInfoResolver =
            new CompositionPolymorphicTypeResolver([typeof(IntegrationEventBase), typeof(IResponseMessage)])
    };
}