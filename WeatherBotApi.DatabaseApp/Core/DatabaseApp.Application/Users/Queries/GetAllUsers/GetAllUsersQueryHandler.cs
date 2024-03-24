using DatabaseApp.Application.Users.Queries.GetUser;
using DatabaseApp.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace DatabaseApp.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler(IUserRepository repository, IMapper mapper)
    : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken) =>
        mapper.Map<List<UserDto>>(await repository.GetAllAsync(cancellationToken));
}