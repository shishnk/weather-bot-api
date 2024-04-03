using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TelegramBotApp.Messaging.IntegrationContext;

public class PolymorphicIntegrationEventTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        var basePointType = typeof(IntegrationEvent);

        if (jsonTypeInfo.Type != basePointType) return jsonTypeInfo;

        var derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p is { IsClass: true, IsAbstract: false })
            .Select(t => new JsonDerivedType(t, t.Name));

        jsonTypeInfo.PolymorphismOptions = new()
        {
            TypeDiscriminatorPropertyName = "$integration-event-type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
        };

        foreach (var jsonDerivedType in derivedTypes)
        {
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(jsonDerivedType);
        }

        return jsonTypeInfo;
    }
}