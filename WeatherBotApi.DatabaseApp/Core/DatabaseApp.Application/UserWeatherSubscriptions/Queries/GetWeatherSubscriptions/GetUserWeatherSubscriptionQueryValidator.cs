using FluentValidation;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;

public class GetUserWeatherSubscriptionsQueryValidator : AbstractValidator<GetUserWeatherSubscriptionsQuery>
{
    public GetUserWeatherSubscriptionsQueryValidator() => RuleFor(x => x.UserTelegramId).GreaterThan(0);
}