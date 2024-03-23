using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

public class UpdateUserWeatherSubscriptionCommand : IRequest
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Location { get; set; }
    public TimeSpan ResendInterval { get; set; }
}