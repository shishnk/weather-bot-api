using DatabaseApp.Domain.Models;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Commands.CreateWeatherDescription;

public class CreateWeatherDescriptionCommand : IRequest<int>
{
    public required Location Location { get; set; }
}