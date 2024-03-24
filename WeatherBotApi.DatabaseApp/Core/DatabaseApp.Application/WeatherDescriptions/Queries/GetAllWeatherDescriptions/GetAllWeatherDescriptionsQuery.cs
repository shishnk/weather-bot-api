using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Queries.GetAllWeatherDescriptions;

public class GetAllWeatherDescriptionsQuery : IRequest<List<WeatherDescriptionDto>>;