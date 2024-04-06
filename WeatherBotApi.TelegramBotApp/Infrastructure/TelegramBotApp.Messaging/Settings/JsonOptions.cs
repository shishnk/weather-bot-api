using System.Text.Json;
using TelegramBotApp.Domain.Responses;
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
        TypeInfoResolverChain =
        {
            new UniversalPolymorphicTypeResolver(typeof(IntegrationEventBase)),
            // new UniversalPolymorphicTypeResolver(typeof(IResponseMessage))
        }
    };
}