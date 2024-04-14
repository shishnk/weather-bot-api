using FluentResults;
using MediatR;

namespace DatabaseApp.Application.Users.Queries.GetUser;

public class GetUserQuery : IRequest<Result<UserDto>>
{
    public required int UserTelegramId { get; init; }
}