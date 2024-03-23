using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

public class CreateUserWeatherSubscriptionCommand : IRequest<int>
{
    public int UserId { get; set; }
    public int WeatherDescriptionId { get; set; }
}