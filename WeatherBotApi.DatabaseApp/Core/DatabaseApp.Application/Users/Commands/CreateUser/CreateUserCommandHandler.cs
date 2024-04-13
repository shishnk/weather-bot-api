using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using FluentResults;
using MediatR;

namespace DatabaseApp.Application.Users.Commands.CreateUser;

// ReSharper disable once UnusedType.Global
public class CreateUserCommandHandler(IUserRepository repository) : IRequestHandler<CreateUserCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await repository.GetByTelegramIdAsync(request.TelegramId, cancellationToken);

        if (existingUser != null)
        {
            return Result.Fail("User already exists");
        }

        var userMetadata = UserMetadata.Create(request.Username, request.MobileNumber);

        if (userMetadata.IsFailed) return userMetadata.ToResult();

        var user = new User
        {
            TelegramId = request.TelegramId,
            Metadata = userMetadata.Value,
            RegisteredAt = request.RegisteredAt
        };

        await repository.AddAsync(user, cancellationToken);
        await repository.SaveDbChangesAsync(cancellationToken);

        return user.TelegramId;
    }
}