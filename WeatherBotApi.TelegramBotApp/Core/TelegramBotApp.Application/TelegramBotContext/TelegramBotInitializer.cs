using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBotApp.Domain.Models;

namespace TelegramBotApp.Application.TelegramBotContext;

public class TelegramBotInitializer : ITelegramBotInitializer
{
#pragma warning disable CA1822
    public ITelegramBot CreateBot(string token, ReceiverOptions receiverOptions)
#pragma warning restore CA1822
    {
        return new TelegramBot(new TelegramBotClient(token), receiverOptions);
    }

#pragma warning disable CA1822
    public ReceiverOptions CreateReceiverOptions() =>
#pragma warning restore CA1822
        new()
        {
            AllowedUpdates = [UpdateType.Message]
        };
}