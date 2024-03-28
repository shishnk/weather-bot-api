using DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;
using DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WeatherSubscriptionController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Creates a new weather subscription for a user.
    /// </summary>
    /// <param name="command">The command containing the subscription data.</param>
    /// <returns>Returns 201 OK if the subscription is successfully created.</returns>
    /// <response code="201">Success</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUserWeatherSubscription(
        [FromBody] CreateUserWeatherSubscriptionCommand command)
    {
        await mediator.Send(command);
        return CreatedAtAction(nameof(GetWeatherSubscriptionsByUserId), new { userId = command.TelegramUserId }, null);
    }

    /// <summary>
    /// Gets weather subscriptions by user ID.
    /// </summary>
    /// <param name="userTelegramId">The ID of the user.</param>
    /// <returns>Returns 200 OK and the list of subscriptions for the user.</returns>
    /// <response code="200">Success</response>
    [HttpGet("{userTelegramId:required:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWeatherSubscriptionsByUserId(int userTelegramId) =>
        Ok(await mediator.Send(new GetUserWeatherSubscriptionsQuery
        {
            UserTelegramId = userTelegramId
        }));

    /// <summary>
    /// Updates a user's weather subscription.
    /// </summary>
    /// <param name="command">The command containing the updated subscription data.</param>
    /// <returns>Returns 204 No Content if the subscription is successfully updated.</returns>
    /// <response code="204">Success</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserWeatherSubscription(
        [FromBody] UpdateUserWeatherSubscriptionCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a user's weather subscription.
    /// </summary>
    /// <param name="command">The command containing the ID of the subscription to delete.</param>
    /// <returns>Returns 204 No Content if the subscription is successfully deleted.</returns>
    /// <response code="204">Success</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUserWeatherSubscription(
        [FromBody] DeleteUserWeatherSubscriptionCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }
}