using DatabaseApp.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Queries.GetAllWeatherDescriptions;

public class GetAllWeatherDescriptionsQueryHandler(IWeatherDescriptionRepository repository, IMapper mapper)
    : IRequestHandler<GetAllWeatherDescriptionsQuery, List<WeatherDescriptionDto>>
{
    public async Task<List<WeatherDescriptionDto>> Handle(GetAllWeatherDescriptionsQuery request,
        CancellationToken cancellationToken)
    {
        return mapper.Map<List<WeatherDescriptionDto>>(await repository.GetAllAsync(cancellationToken));
    }
}