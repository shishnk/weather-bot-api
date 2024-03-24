using DatabaseApp.Domain.Models;
using MediatR;

namespace DatabaseApp.Application.WeatherDescriptions.Commands.UpdateWeatherDescription;

public class UpdateWeatherDescriptionCommand : IRequest
{
    public int Id { get; set; }
    public required Location Location { get; set; }
}