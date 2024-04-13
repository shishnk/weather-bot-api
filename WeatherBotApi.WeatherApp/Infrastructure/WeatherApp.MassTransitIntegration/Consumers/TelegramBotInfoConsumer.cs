using MassTransit;
using Microsoft.Extensions.Logging;
using WeatherApp.Application.Services;
using WeatherApp.Domain.Models;

namespace WeatherApp.MassTransitIntegration.Consumers;

public class TelegramBotInfoConsumer(IWeatherService weatherService, ILogger<TelegramBotInfoConsumer> logger) : IConsumer<TelegramBotInfo>
{
    public async Task Consume(ConsumeContext<TelegramBotInfo> context)
    {
        logger.LogInformation("Received a new message from {User}", context.Message.User);
        var weatherDescriptor = await weatherService.GetWeatherForecastAsync(context.Message.Location);
        await context.RespondAsync(weatherDescriptor);
    }
}