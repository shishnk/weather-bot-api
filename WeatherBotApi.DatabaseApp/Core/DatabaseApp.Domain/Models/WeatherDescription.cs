namespace DatabaseApp.Domain.Models;

public class WeatherDescription : IEntity
{
    public int Id { get; init; }
    public required string Location { get; set; }
}