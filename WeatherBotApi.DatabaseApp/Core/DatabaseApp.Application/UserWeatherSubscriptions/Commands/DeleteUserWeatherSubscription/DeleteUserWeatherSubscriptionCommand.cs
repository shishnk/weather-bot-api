using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

public class DeleteUserWeatherSubscriptionCommand(int userTelegramId, string location) : IRequest<Result>
{
    public int UserTelegramId { get; init; } = userTelegramId;
    public required string Location { get; init; } = location;
}