using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;

public class GetUserWeatherSubscriptionsQuery : IRequest<List<UserWeatherSubscriptionDto>>
{
    public long UserTelegramId { get; init; }
}