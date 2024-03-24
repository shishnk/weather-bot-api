using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using MediatR;

namespace DatabaseApp.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUserRepository repository) : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Username = request.Username,
            MobileNumber = request.MobileNumber,
            RegisteredAt = request.RegisteredAt
        };

        await repository.AddAsync(user, cancellationToken);
        await repository.SaveDbChangesAsync(cancellationToken);

        return user.Id;
    }
}