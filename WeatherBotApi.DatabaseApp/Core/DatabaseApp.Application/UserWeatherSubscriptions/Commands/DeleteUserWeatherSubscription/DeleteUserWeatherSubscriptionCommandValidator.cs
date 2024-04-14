using FluentValidation;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class DeleteUserWeatherSubscriptionCommandValidator : AbstractValidator<DeleteUserWeatherSubscriptionCommand>
{
    public DeleteUserWeatherSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserTelegramId).GreaterThan(0);
        RuleFor(x => x.Location).NotNull().NotEmpty();
    }
}