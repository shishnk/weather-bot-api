using MediatR;

namespace DatabaseApp.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<int>
{
    public required string Username { get; set; }
    public required string MobileNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
}