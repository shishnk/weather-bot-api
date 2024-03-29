using FluentValidation;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

public class UpdateUserWeatherSubscriptionCommandValidator : AbstractValidator<UpdateUserWeatherSubscriptionCommand>
{
    public UpdateUserWeatherSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserTelegramId).GreaterThan(0);
        RuleFor(x => x.Location).NotNull().NotEmpty();
        RuleFor(x => x.ResendInterval).GreaterThan(TimeSpan.Zero);
    }
}