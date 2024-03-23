using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;

public class GetUserWeatherSubscriptionsQuery : IRequest<List<UserWeatherSubscriptionDto>>
{
    public int UserId { get; set; }
}