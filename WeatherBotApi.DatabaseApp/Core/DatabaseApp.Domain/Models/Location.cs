using FluentResults;

namespace DatabaseApp.Domain.Models;

public record Location
{
    public string Value { get; private set; }

    private Location(string value) => Value = value;

    public static Result<Location> Create(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Fail<Location>("Location cannot be null or whitespace.")
            : Result.Ok(new Location(value));
}