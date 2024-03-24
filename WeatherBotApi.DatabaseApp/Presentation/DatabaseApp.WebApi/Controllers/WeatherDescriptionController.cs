using DatabaseApp.Application.WeatherDescriptions.Commands.CreateWeatherDescription;
using DatabaseApp.Application.WeatherDescriptions.Commands.UpdateWeatherDescription;
using DatabaseApp.Application.WeatherDescriptions.Queries.GetAllWeatherDescriptions;
using DatabaseApp.Application.WeatherDescriptions.Queries.GetWeatherDescription;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WeatherDescriptionController(ISender meditor) : ControllerBase
{
    /// <summary>
    /// Gets all weather descriptions.
    /// </summary>
    /// <returns>Returns 200 OK and a list of all weather descriptions.</returns>
    /// <response code="200">Success</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WeatherDescriptionDto>>> GetAllWeatherDescriptions() =>
        Ok(await meditor.Send(new GetAllWeatherDescriptionsQuery()));

    /// <summary>
    /// Gets a weather description by ID.
    /// </summary>
    /// <param name="id">The ID of the weather description.</param>
    /// <returns>Returns 200 OK and the weather description with the specified ID.</returns>
    /// <response code="200">Success</response>
    [HttpGet("{id:required:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<WeatherDescriptionDto>> GetWeatherDescription(int id) =>
        Ok(await meditor.Send(new GetWeatherDescriptionQuery
        {
            Id = id
        }));

    /// <summary>
    /// Creates a new weather description.
    /// </summary>
    /// <param name="command">The command containing the weather description data.</param>
    /// <returns>Returns 201 Created if the weather description is successfully created.</returns>
    /// <response code="201">Success</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWeatherDescription([FromBody] CreateWeatherDescriptionCommand command)
    {
        var id = await meditor.Send(command);
        return CreatedAtAction(nameof(GetWeatherDescription), new { id }, null);
    }

    /// <summary>
    /// Updates a weather description.
    /// </summary>
    /// <param name="command">The command containing the updated weather description data.</param>
    /// <returns>Returns 204 No Content if the weather description is successfully updated.</returns>
    /// <response code="204">Success</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateWeatherDescription([FromBody] UpdateWeatherDescriptionCommand command)
    {
        await meditor.Send(command);
        return NoContent();
    }
}