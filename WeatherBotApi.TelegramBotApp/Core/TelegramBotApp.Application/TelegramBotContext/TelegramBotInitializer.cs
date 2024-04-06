using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TelegramBotApp.Application.TelegramBotContext;

public class TelegramBotInitializer : IBotInitializer
{
    public ITelegramBot CreateBot(string token)
    {
        return new TelegramBot(new TelegramBotClient(token));
    }

    public ReceiverOptions CreateReceiverOptions() =>
        new()
        {
            AllowedUpdates = [UpdateType.Message]
        };
}