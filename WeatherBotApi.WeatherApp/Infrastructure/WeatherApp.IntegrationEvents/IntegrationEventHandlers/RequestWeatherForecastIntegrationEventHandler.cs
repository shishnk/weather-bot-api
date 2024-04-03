using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;
using WeatherApp.Application.Services;

namespace WeatherApp.IntegrationEvents.IntegrationEventHandlers;

// ReSharper disable once ClassNeverInstantiated.Global
public class RequestWeatherForecastIntegrationEventHandler(IWeatherService weatherService)
    : IIntegrationEventHandler<RequestWeatherForecastIntegrationEvent>
{
    public async Task<IResponseMessage?> Handle(RequestWeatherForecastIntegrationEvent @event)
    {
        var weatherForecast = await weatherService.GetWeatherForecastAsync(@event.Location);
        return new Kek
        {
            Value = weatherForecast.FeelTemperature.ToString()
        };
    }
}