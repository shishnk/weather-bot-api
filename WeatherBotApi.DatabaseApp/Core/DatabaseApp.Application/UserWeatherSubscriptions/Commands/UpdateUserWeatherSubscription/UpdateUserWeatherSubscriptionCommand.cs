using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

public class UpdateUserWeatherSubscriptionCommand(TimeSpan resendInterval, string location) : IRequest<Result>
{
    public int UserTelegramId { get; init; }
    public required string Location { get; init; } = location;
    public TimeSpan ResendInterval { get; } = resendInterval;
}