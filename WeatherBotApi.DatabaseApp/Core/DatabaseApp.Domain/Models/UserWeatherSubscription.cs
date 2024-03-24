namespace DatabaseApp.Domain.Models;

public class UserWeatherSubscription : IEntity
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public TimeSpan ResendInterval { get; set; }
    public required Location Location { get; set; }
    public User User { get; init; } = null!;
}