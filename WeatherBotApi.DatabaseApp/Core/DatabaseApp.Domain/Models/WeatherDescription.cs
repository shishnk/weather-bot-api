namespace DatabaseApp.Domain.Models;

public class WeatherDescription : IEntity
{
    public int Id { get; init; }
    public required Location Location { get; init; }

    public void UpdateLocation(string value) => Location.Update(value);
}