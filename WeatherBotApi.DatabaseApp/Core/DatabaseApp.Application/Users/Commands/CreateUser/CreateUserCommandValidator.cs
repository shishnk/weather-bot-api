using DatabaseApp.Domain.Models;
using FluentValidation;

namespace DatabaseApp.Application.Users.Commands.CreateUser;

// ReSharper disable once UnusedType.Global
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.TelegramId).GreaterThan(0);
        RuleFor(x => x.Username).NotEmpty().MaximumLength(UserMetadata.MaxUsernameLength);
        RuleFor(x => x.MobileNumber).NotEmpty();
        RuleFor(x => x.RegisteredAt).NotEmpty();
    }
}