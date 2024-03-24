using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

public class CreateUserWeatherSubscriptionCommand : IRequest, IRequest<Result>
{
    public int TelegramUserId { get; set; }
    public required string Location { get; set; }
    public TimeSpan ResendInterval { get; set; }
}