using DatabaseApp.Application.Users.Queries.GetUser;
using MediatR;

namespace DatabaseApp.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<UserDto>>;