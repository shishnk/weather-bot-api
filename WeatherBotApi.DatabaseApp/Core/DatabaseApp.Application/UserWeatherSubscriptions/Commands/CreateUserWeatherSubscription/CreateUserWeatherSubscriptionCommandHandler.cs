using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;

public class CreateUserWeatherSubscriptionCommandHandler(IWeatherSubscriptionRepository repository)
    : IRequestHandler<CreateUserWeatherSubscriptionCommand, int>
{
    public async Task<int> Handle(CreateUserWeatherSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var weatherSubscription = new UserWeatherSubscription
        {
            UserId = request.UserId,
            WeatherDescriptionId = request.WeatherDescriptionId
        };

        await repository.AddAsync(weatherSubscription, cancellationToken);
        await repository.SaveDbChangesAsync(cancellationToken);

        return weatherSubscription.Id;
    }
}