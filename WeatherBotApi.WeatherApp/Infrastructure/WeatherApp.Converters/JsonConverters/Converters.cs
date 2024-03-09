using System.Text.Json;
using System.Text.Json.Serialization;
using WeatherApp.Domain.Models;

namespace Converters.JsonConverters;

public class WeatherDescriptorJsonConverter : JsonConverter<WeatherDescriptor>
{
    public override WeatherDescriptor Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;

        var jElement = root.GetProperty("current_condition");

        if (jElement.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("No current_condition array");
        }

        var mainToken = jElement[0];

        if (!int.TryParse(mainToken.GetProperty("humidity").GetString(), out var humidity))
        {
            throw new JsonException("Invalid value for humidity");
        }

        if (!int.TryParse(mainToken.GetProperty("temp_C").GetString(), out var temperature))
        {
            throw new JsonException("Invalid value for temp_C");
        }

        if (!int.TryParse(mainToken.GetProperty("FeelsLikeC").GetString(), out var feelTemperature))
        {
            throw new JsonException("Invalid value for FeelsLikeC");
        }

        if (!int.TryParse(mainToken.GetProperty("pressureInches").GetString(), out var pressure))
        {
            throw new JsonException("Invalid value for pressureInches");
        }

        if (!int.TryParse(mainToken.GetProperty("visibilityMiles").GetString(), out var visibilityMiles))
        {
            throw new JsonException("Invalid value for visibilityMiles");
        }

        if (!int.TryParse(mainToken.GetProperty("windspeedKmph").GetString(), out var wind))
        {
            throw new JsonException("Invalid value for windspeedKmph");
        }

        if (!int.TryParse(mainToken.GetProperty("uvIndex").GetString(), out var uvIndex))
        {
            throw new JsonException("Invalid value for uvIndex");
        }

        return new()
        {
            Humidity = humidity,
            Temperature = temperature,
            FeelTemperature = feelTemperature,
            Pressure = pressure,
            Visibility = visibilityMiles,
            Wind = wind,
            UvIndex = uvIndex
        };
    }

    public override void Write(Utf8JsonWriter writer, WeatherDescriptor value, JsonSerializerOptions options) =>
        throw new NotImplementedException();
}