using DatabaseApp.Application.WeatherDescriptions.Queries.GetAllWeatherDescriptions;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Queries.GetWeatherDescription;

public class GetWeatherDescriptionQuery : IRequest<WeatherDescriptionDto>
{
    public required int Id { get; set; }
}