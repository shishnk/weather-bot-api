using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TelegramBotApp.Messaging;

public class CompositionPolymorphicTypeResolver(IEnumerable<Type> baseTypes) : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        
        var baseType = baseTypes.FirstOrDefault(bt => bt.IsAssignableFrom(jsonTypeInfo.Type));
        
        if (baseType == null) return jsonTypeInfo;
    
        var derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p is { IsClass: true, IsAbstract: false })
            .Select(t => new JsonDerivedType(t, t.Name));

        jsonTypeInfo.PolymorphismOptions = new()
        {
            TypeDiscriminatorPropertyName = $"{jsonTypeInfo.Type.Name.ToLower()}-type",
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