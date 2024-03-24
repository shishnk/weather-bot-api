using DatabaseApp.Domain.Models;

namespace DatabaseApp.Application.WeatherDescriptions.Queries.GetAllWeatherDescriptions;

public class WeatherDescriptionDto
{
    public required Location Location { get; init; }
}