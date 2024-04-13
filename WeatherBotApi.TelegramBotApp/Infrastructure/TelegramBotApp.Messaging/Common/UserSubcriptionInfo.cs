namespace TelegramBotApp.Messaging.Common;

public class UserSubscriptionInfo
{
    public long TelegramId { get; init; }
    public TimeSpan ResendInterval { get; init; }
    public required string Location { get; init; }
}