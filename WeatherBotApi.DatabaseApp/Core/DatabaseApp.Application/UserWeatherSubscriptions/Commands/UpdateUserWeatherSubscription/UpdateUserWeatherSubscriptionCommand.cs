using DatabaseApp.Domain.Models;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

public class UpdateUserWeatherSubscriptionCommand : IRequest
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required Location Location { get; set; }
    public TimeSpan ResendInterval { get; set; }
}