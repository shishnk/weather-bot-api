using DatabaseApp.Application.Common.Exceptions;
using DatabaseApp.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace DatabaseApp.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null) throw new NotFoundException(nameof(user), request.Id);

        return mapper.Map<UserDto>(user);
    }
}