namespace DatabaseApp.Domain.Models;

public class UserWeatherSubscription
{
    public int Id { get; init; }
    public int WeatherDescriptionId { get; init; }
    public int UserId { get; init; }
    public WeatherDescription WeatherDescription { get; init; } = null!;
    public User User { get; init; } = null!;
}