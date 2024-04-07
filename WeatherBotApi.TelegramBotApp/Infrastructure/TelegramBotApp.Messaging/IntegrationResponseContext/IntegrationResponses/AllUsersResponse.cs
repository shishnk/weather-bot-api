namespace TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

public class AllUsersResponse : IResponseMessage
{
    public required List<int> UserTelegramIds { get; init; }
}