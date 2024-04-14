using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;
using WeatherApp.Application.MessageFormatters;
using WeatherApp.Application.Services;

namespace WeatherApp.IntegrationEvents.IntegrationEventHandlers;

// ReSharper disable once ClassNeverInstantiated.Global
public class WeatherForecastRequestIntegrationEventHandler(IWeatherService weatherService)
    : IIntegrationEventHandler<WeatherForecastRequestIntegrationEvent>
{
    private readonly WeatherDescriptorFormatter _messageFormatter = new();

    public async Task<IResponseMessage> Handle(WeatherForecastRequestIntegrationEvent @event)
    {
        try
        {
            var weatherDescriptor = await weatherService.GetWeatherForecastAsync(@event.Location);
            weatherDescriptor.SetLocation(@event.Location);
            return new UniversalResponse(_messageFormatter.Format(weatherDescriptor));
        }
        catch (HttpRequestException)
        {
            return new UniversalResponse($"Failed to get the weather forecast. Bad location '{@event.Location}' or network issues.");
        }
        catch (Exception e)
        {
            return new UniversalResponse(e.Message);
        }
    }
}