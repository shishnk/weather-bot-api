namespace TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

public class CreatedUserIntegrationEvent : IntegrationEvent
{
    public override string Name => nameof(CreatedUserIntegrationEvent);
    
    public int UserTelegramId { get; init; }
    public required string Username { get; init; }
    public required string MobileNumber { get; init; }
    public DateTime RegisteredAt { get; init; }
}