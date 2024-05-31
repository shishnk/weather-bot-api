using Microsoft.AspNetCore.Mvc;
using WeatherApp.WeatherStation.Services;

namespace WeatherApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger) : ControllerBase
{
    [HttpGet("{location:required}")]
    public async Task<IActionResult> GetWeatherForecast(string location)
    {
        try
        {
            logger.LogInformation("Getting weather forecast for {location}", location);
            var descriptor = await weatherService.GetWeatherForecastAsync(location);

            logger.LogInformation("Weather forecast for {Location} retrieved successfully", location);
            return Ok(descriptor);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to retrieve weather forecast for {Location}", location);
            return BadRequest(e.Message);
        }
    }
}