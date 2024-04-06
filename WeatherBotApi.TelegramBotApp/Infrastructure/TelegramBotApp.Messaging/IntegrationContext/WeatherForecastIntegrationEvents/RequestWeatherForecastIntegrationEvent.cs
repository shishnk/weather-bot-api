using System.Text.Json.Serialization;

namespace TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;

[method: JsonConstructor]
public class WeatherForecastRequestIntegrationEvent(string location) : IntegrationEventBase
{
    public override string Name => nameof(WeatherForecastRequestIntegrationEvent);
    public string Location => location;
}