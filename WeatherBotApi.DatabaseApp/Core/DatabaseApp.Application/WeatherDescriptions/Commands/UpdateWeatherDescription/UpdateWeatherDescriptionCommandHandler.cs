using DatabaseApp.Application.Common.Exceptions;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Commands.UpdateWeatherDescription;

public class UpdateWeatherDescriptionCommandHandler(IWeatherDescriptionRepository repository)
    : IRequestHandler<UpdateWeatherDescriptionCommand>
{
    public async Task Handle(UpdateWeatherDescriptionCommand request, CancellationToken cancellationToken)
    {
        var weatherDescription = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (weatherDescription == null) throw new NotFoundException(nameof(weatherDescription), request.Location);

        weatherDescription.UpdateLocation(request.Location.Value);

        repository.Update(weatherDescription);
        await repository.SaveDbChangesAsync(cancellationToken);
    }
}