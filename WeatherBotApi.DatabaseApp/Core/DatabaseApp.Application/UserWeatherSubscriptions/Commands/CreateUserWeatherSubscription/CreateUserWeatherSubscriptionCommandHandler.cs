using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class CreateUserWeatherSubscriptionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserWeatherSubscriptionCommand, Result>
{
    public async Task<Result> Handle(CreateUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var location = Location.Create(request.Location);

        if (location.IsFailed) return location.ToResult();

        var existingSubscription = await unitOfWork.UserWeatherSubscriptionRepository
            .GetByUserTelegramIdAndLocationAsync(request.TelegramUserId, location.Value, cancellationToken);

        if (existingSubscription != null) return Result.Fail(new Error("Weather subscription already exists"));

        var user = await unitOfWork.UserRepository.GetByTelegramIdAsync(request.TelegramUserId, cancellationToken);

        if (user == null) return Result.Fail(new Error("User not found"));

        var weatherSubscription = new UserWeatherSubscription
        {
            UserId = user.Id,
            Location = location.Value,
            ResendInterval = request.ResendInterval
        };

        await unitOfWork.UserWeatherSubscriptionRepository.AddAsync(weatherSubscription, cancellationToken);
        await unitOfWork.SaveDbChangesAsync(cancellationToken);

        return Result.Ok();
    }
}