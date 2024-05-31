using System.Text.Json;
using WeatherApp.Domain.Models;

namespace WeatherApp.WeatherStation.Services;

public class WeatherService(HttpClient httpClient, JsonSerializerOptions options) : IWeatherService
{
    private const string Url = "https://wttr.in/";

    public async Task<WeatherDescriptor> GetWeatherForecastAsync(string location)
    {
        var url = Path.Combine(Url, location + "?format=j1");

        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync();
        var descriptor = await JsonSerializer.DeserializeAsync<WeatherDescriptor>(stream, options);

        if (descriptor == null) throw new InvalidOperationException("Failed to deserialize the weather descriptor");

        return descriptor;
    }
}