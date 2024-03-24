using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class UpdateUserWeatherSubscriptionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserWeatherSubscriptionCommand, Result>
{
    public async Task<Result> Handle(UpdateUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var location = Location.Create(request.Location);

        if (location.IsFailed) return location.ToResult();

        var user = await unitOfWork.UserRepository.GetByTelegramIdAsync(request.UserTelegramId, cancellationToken);

        if (user == null) return Result.Fail(new Error("User not found"));

        var subscription =
            await unitOfWork.UserWeatherSubscriptionRepository.GetByUserTelegramIdAndLocationAsync(
                request.UserTelegramId, location.Value, cancellationToken);

        if (subscription == null) return Result.Fail(new Error("Subscription not found"));

        subscription.Location = location.Value;
        subscription.ResendInterval = request.ResendInterval;

        unitOfWork.UserWeatherSubscriptionRepository.Update(subscription);
        await unitOfWork.SaveDbChangesAsync(cancellationToken);

        return Result.Ok();
    }
}