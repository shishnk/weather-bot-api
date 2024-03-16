namespace WeatherApp.Domain.Models;

public class TelegramBotWeatherRequest
{
    public string Location { get; init; }
    public string User { get; init; }
}