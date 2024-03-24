using DatabaseApp.Application.Common.Exceptions;
using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class CreateUserWeatherSubscriptionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserWeatherSubscriptionCommand, int>
{
    public async Task<int> Handle(CreateUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var weatherDescription =
            await unitOfWork.WeatherDescriptionRepository.GetByLocationAsync(request.Location, cancellationToken);

        if (weatherDescription == null) throw new NotFoundException(nameof(weatherDescription), request.Location);

        var weatherSubscription = new UserWeatherSubscription
        {
            UserId = request.UserId,
            WeatherDescriptionId = weatherDescription.Id,
            ResendInterval = request.ResendInterval
        };

        await unitOfWork.UserWeatherSubscriptionRepository.AddAsync(weatherSubscription, cancellationToken);
        await unitOfWork.SaveDbChangesAsync(cancellationToken);

        return weatherSubscription.Id;
    }
}