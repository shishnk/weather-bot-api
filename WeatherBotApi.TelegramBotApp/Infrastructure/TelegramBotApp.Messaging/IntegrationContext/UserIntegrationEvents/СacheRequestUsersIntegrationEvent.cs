namespace TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

public class CacheRequestUsersIntegrationEvent : IntegrationEventBase
{
    public override string Name => nameof(CacheRequestUsersIntegrationEvent);
}