namespace TelegramBotApp.Domain.Models;

public class WeatherRequest : IResponseMessage
{
    public string Name => nameof(WeatherRequest);
    public string Value { get; }
    public string Location { get; init; }
    public string User { get; init; }
}