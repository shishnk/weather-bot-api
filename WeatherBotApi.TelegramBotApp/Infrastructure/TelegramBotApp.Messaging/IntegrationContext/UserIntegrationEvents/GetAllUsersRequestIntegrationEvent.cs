namespace TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

public class GetAllUsersRequestIntegrationEvent : IntegrationEventBase
{
    public override string Name => nameof(GetAllUsersRequestIntegrationEvent);
}