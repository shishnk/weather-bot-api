namespace DatabaseApp.Domain.Models;

public class WeatherDescription
{
    public int Id { get; init; }
    public required string Location { get; init; }
}