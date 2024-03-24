using DatabaseApp.Application.Common.Exceptions;
using DatabaseApp.Application.WeatherDescriptions.Queries.GetAllWeatherDescriptions;
using DatabaseApp.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Queries.GetWeatherDescription;

public class GetWeatherDescriptionQueryHandler(IWeatherDescriptionRepository repository, IMapper mapper)
    : IRequestHandler<GetWeatherDescriptionQuery, WeatherDescriptionDto>
{
    public async Task<WeatherDescriptionDto> Handle(GetWeatherDescriptionQuery request,
        CancellationToken cancellationToken)
    {
        var weatherDescription = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (weatherDescription == null) throw new NotFoundException(nameof(weatherDescription), request.Id);

        return mapper.Map<WeatherDescriptionDto>(weatherDescription);
    }
}