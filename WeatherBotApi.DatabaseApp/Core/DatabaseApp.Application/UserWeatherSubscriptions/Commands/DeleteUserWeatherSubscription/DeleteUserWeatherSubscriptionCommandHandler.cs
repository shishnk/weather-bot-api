using DatabaseApp.Application.Common.Exceptions;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

// ReSharper disable once UnusedType.Global
public class DeleteUserWeatherSubscriptionCommandHandler(IUnitOfWork unitOfWork)
    : IRequest<DeleteUserWeatherSubscriptionCommand>
{
    // ReSharper disable once UnusedMember.Global
    public async Task Handle(DeleteUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var weatherDescription =
            await unitOfWork.WeatherDescriptionRepository.GetByLocationAsync(request.Location, cancellationToken);

        if (weatherDescription == null) throw new NotFoundException(nameof(weatherDescription), request.Location);

        var subscription =
            await unitOfWork.UserWeatherSubscriptionRepository.GetByIdAsync(weatherDescription.Id, cancellationToken);

        if (subscription == null) throw new NotFoundException(nameof(subscription), weatherDescription.Id);

        unitOfWork.UserWeatherSubscriptionRepository.Delete(subscription);
        await unitOfWork.SaveDbChangesAsync(cancellationToken);
    }
}