using DatabaseApp.Application.Users.Commands.CreateUser;
using DatabaseApp.Application.Users.Queries.GetAllUsers;
using DatabaseApp.Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController(ISender mediator) : ControllerBase
{
    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>Returns 200 OK and a list of all users.</returns>
    /// <response code="200">Success</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers() =>
        Ok(await mediator.Send(new GetAllUsersQuery()));

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>Returns 200 OK and the user with the specified ID.</returns>
    /// <response code="200">Success</response>
    [HttpGet("{id:required:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser(int id) =>
        Ok(await mediator.Send(new GetUserQuery
        {
            UserTelegramId = id
        }));

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="command">The command containing the user data.</param>
    /// <returns>Returns 201 Created if the user is successfully created.</returns>
    /// <response code="201">Success</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id }, null);
    }
}