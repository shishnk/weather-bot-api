using WeatherApp.Domain.Models;

namespace WeatherApp.Application.Services;

public interface IWeatherService
{
    Task<WeatherDescriptor> GetWeatherForecastAsync(string location);
}
