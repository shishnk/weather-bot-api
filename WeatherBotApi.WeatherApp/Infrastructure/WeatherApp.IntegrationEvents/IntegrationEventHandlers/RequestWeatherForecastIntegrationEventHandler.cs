using TelegramBotApp.Domain.Responses;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;
using WeatherApp.Application.MessageFormatters;
using WeatherApp.Application.Services;

namespace WeatherApp.IntegrationEvents.IntegrationEventHandlers;

// ReSharper disable once ClassNeverInstantiated.Global
public class WeatherForecastRequestIntegrationEventHandler(IWeatherService weatherService)
    : IIntegrationEventHandler<WeatherForecastRequestIntegrationEvent>
{
    private readonly WeatherDescriptorFormatter _messageFormatter = new();

    public async Task<UniversalResponse?> Handle(WeatherForecastRequestIntegrationEvent eventBase) =>
        new(_messageFormatter.Format(await weatherService.GetWeatherForecastAsync(eventBase.Location)));
}