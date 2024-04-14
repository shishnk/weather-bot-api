namespace TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;

public class UpdatedUserSubscriptionIntegrationEvent : IntegrationEventBase
{
    public override string Name => nameof(UpdatedUserSubscriptionIntegrationEvent);
    public long TelegramUserId { get; init; }
    public required string Location { get; init; }
    public TimeSpan ResendInterval { get; init; }
}