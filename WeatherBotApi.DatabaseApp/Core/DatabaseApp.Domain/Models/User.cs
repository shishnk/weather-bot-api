namespace DatabaseApp.Domain.Models;

public class User : IEntity
{
    public int Id { get; init; }
    public required string Username { get; init; }
    public required string MobileNumber { get; init; }
    public DateTime RegisteredAt { get; init; }
}