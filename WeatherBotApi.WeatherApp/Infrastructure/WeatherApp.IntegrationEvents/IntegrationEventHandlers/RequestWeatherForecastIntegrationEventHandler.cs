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

    public async Task<IResponseMessage?> Handle(WeatherForecastRequestIntegrationEvent eventBase) =>
        new UniversalResponse(
            _messageFormatter.Format(await weatherService.GetWeatherForecastAsync(eventBase.Location)));
}