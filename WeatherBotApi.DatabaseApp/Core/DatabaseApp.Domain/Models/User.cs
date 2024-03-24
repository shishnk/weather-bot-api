namespace DatabaseApp.Domain.Models;

public class User : IEntity
{
    public int Id { get; init; }
    public int TelegramId { get; init; }
    public required UserMetadata Metadata { get; init; }
    public DateTime RegisteredAt { get; init; }
}