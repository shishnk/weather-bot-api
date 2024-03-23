namespace DatabaseApp.Domain.Models;

public class UserWeatherSubscription : IEntity
{
    public int Id { get; init; }
    public int WeatherDescriptionId { get; init; }
    public int UserId { get; init; }
    public TimeSpan ResendInterval { get; set; }
    public WeatherDescription WeatherDescription { get; init; } = null!;
    public User User { get; init; } = null!;
}