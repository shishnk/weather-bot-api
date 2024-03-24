using MediatR;

namespace DatabaseApp.Application.Users.Queries.GetUser;

public class GetUserQuery : IRequest<UserDto>
{
    public required int Id { get; set; }
}