namespace TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;

public class CreatedUserSubscriptionIntegrationEvent : IntegrationEventBase
{
    public override string Name => nameof(CreatedUserSubscriptionIntegrationEvent);
    public long TelegramId { get; init; }
    public required string City { get; init; }
    public TimeSpan ResendInterval { get; init; }
}