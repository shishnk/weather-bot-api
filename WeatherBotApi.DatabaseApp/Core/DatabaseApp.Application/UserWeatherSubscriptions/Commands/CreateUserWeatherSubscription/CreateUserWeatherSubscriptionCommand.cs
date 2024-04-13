using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

public class CreateUserWeatherSubscriptionCommand : IRequest<Result>
{
    public long TelegramUserId { get; init; }
    public required string Location { get; init; }
    public TimeSpan ResendInterval { get; init; }
}