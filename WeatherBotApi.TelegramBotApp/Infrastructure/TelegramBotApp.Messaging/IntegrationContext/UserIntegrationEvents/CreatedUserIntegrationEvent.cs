namespace TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

public class CreatedUserIntegrationEventBase : IntegrationEventBase
{
    public override string Name => nameof(CreatedUserIntegrationEventBase);
    
    public int UserTelegramId { get; init; }
    public required string Username { get; init; }
    public required string MobileNumber { get; init; }
    public DateTime RegisteredAt { get; init; }
}