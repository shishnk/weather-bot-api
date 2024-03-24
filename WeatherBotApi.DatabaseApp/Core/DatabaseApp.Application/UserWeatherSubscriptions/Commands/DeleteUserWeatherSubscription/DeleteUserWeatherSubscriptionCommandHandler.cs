using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class DeleteUserWeatherSubscriptionCommandHandler(IWeatherSubscriptionRepository repository)
    : IRequest<DeleteUserWeatherSubscriptionCommand>
{
    // ReSharper disable once UnusedMember.Global
    public async Task<Result> Handle(DeleteUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var location = Location.Create(request.Location);

        if (location.IsFailed) return location.ToResult();

        var subscription =
            await repository.GetByUserTelegramIdAndLocationAsync(request.UserTelegramId, location.Value,
                cancellationToken);

        if (subscription == null) return Result.Fail(new Error("Subscription not found"));

        repository.Delete(subscription);
        await repository.SaveDbChangesAsync(cancellationToken);

        return Result.Ok();
    }
}