using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

public class UpdateUserWeatherSubscriptionCommand : IRequest<Result>
{
    public int UserTelegramId { get; init; }
    public required string Location { get; init; }
    public TimeSpan ResendInterval { get; init; }
}