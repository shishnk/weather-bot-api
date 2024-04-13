using TelegramBotApp.Messaging;
using WeatherApp.Domain.Models;

namespace WeatherApp.Application.MessageFormatters;

public class WeatherDescriptorFormatter : IMessageFormatter<WeatherDescriptor>
{
    public string Format(WeatherDescriptor value) =>
        $"""
         Location: {value.Location}
         Temperature: {value.Temperature}°C
         Feels Like: {value.FeelTemperature}°C
         Humidity: {value.Humidity}%
         Pressure: {value.Pressure} hPa
         Visibility: {value.Visibility} m
         Wind Speed: {value.Wind} km/h
         UV Index: {value.UvIndex}
         """;
}