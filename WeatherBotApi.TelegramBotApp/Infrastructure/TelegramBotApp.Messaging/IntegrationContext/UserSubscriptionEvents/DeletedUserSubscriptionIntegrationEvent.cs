namespace TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;

public class DeletedUserSubscriptionIntegrationEvent : IntegrationEventBase
{
    public override string Name => nameof(DeletedUserSubscriptionIntegrationEvent);
    public long TelegramUserId { get; init; }
    public required string Location { get; init; }
}