namespace TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

public class CreatedUserIntegrationEvent : IntegrationEventBase
{
    public override string Name => nameof(CreatedUserIntegrationEvent);
    
    public long UserTelegramId { get; init; }
    public required string Username { get; init; }
    public required string MobileNumber { get; init; }
    public DateTime RegisteredAt { get; init; }
}