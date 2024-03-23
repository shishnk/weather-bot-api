using DatabaseApp.Application.Common.Exceptions;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;

public class UpdateUserWeatherSubscriptionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserWeatherSubscriptionCommand>
{
    public async Task Handle(UpdateUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var weatherDescription =
            await unitOfWork.WeatherDescriptionRepository.GetByLocationAsync(request.Location, cancellationToken);

        if (weatherDescription == null) throw new NotFoundException(nameof(weatherDescription), request.Location);

        var subscription =
            await unitOfWork.UserWeatherSubscriptionRepository.GetByIdAsync(weatherDescription.Id, cancellationToken);

        if (subscription == null) throw new NotFoundException(nameof(subscription), weatherDescription.Id);

        weatherDescription.Location = request.Location;
        subscription.ResendInterval = request.ResendInterval;

        unitOfWork.UserWeatherSubscriptionRepository.Update(subscription);
        await unitOfWork.SaveDbChangesAsync(cancellationToken);
    }
}