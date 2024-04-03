using System.Text.Json.Serialization;

namespace TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;

[method: JsonConstructor]
public class RequestWeatherForecastIntegrationEvent(string location) : IntegrationEvent
{
    public override string Name => nameof(RequestWeatherForecastIntegrationEvent);
    public string Location => location;
}