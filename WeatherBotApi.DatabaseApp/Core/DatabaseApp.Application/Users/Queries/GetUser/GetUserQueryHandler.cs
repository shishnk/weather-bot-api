using DatabaseApp.Domain.Repositories;
using FluentResults;
using MapsterMapper;
using MediatR;

namespace DatabaseApp.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(IUserRepository repository, IMapper mapper)
    : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByTelegramIdAsync(request.UserTelegramId, cancellationToken);

        if (user == null) return Result.Fail(new Error("User not found"));

        return mapper.Map<UserDto>(user);
    }
}