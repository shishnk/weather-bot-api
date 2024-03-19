namespace TelegramBotApp.Domain.Models;

public class WeatherRequest : IResponseMessage
{
    public string Location { get; init; }
    public string User { get; init; }
}