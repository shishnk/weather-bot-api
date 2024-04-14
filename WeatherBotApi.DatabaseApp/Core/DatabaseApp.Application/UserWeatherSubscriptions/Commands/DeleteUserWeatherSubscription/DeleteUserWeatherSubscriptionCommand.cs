using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

public class DeleteUserWeatherSubscriptionCommand : IRequest<Result>
{
    public long UserTelegramId { get; init; }
    public required string Location { get; init; }
}