using DatabaseApp.Domain.Models;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

public class CreateUserWeatherSubscriptionCommand : IRequest<int>
{
    public int UserId { get; set; }
    public required Location Location { get; set; }
    public TimeSpan ResendInterval { get; set; }
}