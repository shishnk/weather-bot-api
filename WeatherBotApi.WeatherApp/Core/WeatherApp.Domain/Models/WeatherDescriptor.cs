namespace WeatherApp.Domain.Models;

public class WeatherDescriptor
{
    public string? Location { get; private set; }
    public required int Temperature { get; init; }
    public required int FeelTemperature { get; init; }
    public required int Humidity { get; init; }
    public required int Pressure { get; init; }
    public required int Visibility { get; init; }
    public required int Wind { get; init; }
    public required int UvIndex { get; init; }

    public void SetLocation(string? location) => Location = location;
}