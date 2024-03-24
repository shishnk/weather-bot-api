using FluentValidation;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class CreateUserWeatherSubscriptionCommandValidator : AbstractValidator<CreateUserWeatherSubscriptionCommand>
{
    public CreateUserWeatherSubscriptionCommandValidator()
    {
        RuleFor(x => x.TelegramUserId).GreaterThan(0);
        RuleFor(x => x.Location).NotNull().NotEmpty();
        RuleFor(x => x.ResendInterval).GreaterThan(TimeSpan.Zero);
    }
}