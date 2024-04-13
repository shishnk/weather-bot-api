using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

public class DeleteUserWeatherSubscriptionCommand : IRequest<Result>
{
    public int UserTelegramId { get; init; }
    public required string Location { get; init; }
}