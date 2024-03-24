using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Commands.CreateWeatherDescription;

public class CreateWeatherDescriptionCommandHandler(IWeatherDescriptionRepository repository)
    : IRequestHandler<CreateWeatherDescriptionCommand, int>
{
    public async Task<int> Handle(CreateWeatherDescriptionCommand request, CancellationToken cancellationToken)
    {
        var weatherDescription = new WeatherDescription
        {
            Location = request.Location
        };

        await repository.AddAsync(weatherDescription, cancellationToken);
        await repository.SaveDbChangesAsync(cancellationToken);

        return weatherDescription.Id;
    }
}