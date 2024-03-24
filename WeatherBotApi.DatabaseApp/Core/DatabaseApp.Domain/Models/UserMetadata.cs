using FluentResults;

namespace DatabaseApp.Domain.Models;

public class UserMetadata
{
    public const int MaxUsernameLength = 100;
    
    public required string Username { get; init; }
    public required string Number { get; init; }

    public static Result<UserMetadata> Create(string username, string number)
    {
        var validationResult = Result.Merge(
            ValidateUsername(username),
            ValidateNumber(number));

        if (validationResult.IsFailed)
            return validationResult;

        return Result.Ok(new UserMetadata
        {
            Username = username,
            Number = number
        });
    }

    private static Result ValidateNumber(string number) =>
        Result.FailIf(string.IsNullOrEmpty(number), "Number is required.");

    private static Result ValidateUsername(string username) =>
        Result.Merge(
            Result.FailIf(string.IsNullOrEmpty(username), "Username is required."),
            Result.FailIf(!string.IsNullOrEmpty(username) && username.Length > MaxUsernameLength,
                "Username should contain max 100 characters.")
        );
}