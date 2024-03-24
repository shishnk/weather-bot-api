using FluentValidation;

namespace DatabaseApp.Application.Users.Queries.GetUser;

// ReSharper disable once UnusedType.Global
public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator() => RuleFor(x => x.UserTelegramId).GreaterThan(0);
}