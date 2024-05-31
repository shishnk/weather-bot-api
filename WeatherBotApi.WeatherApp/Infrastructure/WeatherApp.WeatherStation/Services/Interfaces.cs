using WeatherApp.Domain.Models;

namespace WeatherApp.WeatherStation.Services;

public interface IWeatherService
{
    Task<WeatherDescriptor> GetWeatherForecastAsync(string location);
}
