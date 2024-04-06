using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotApp.Messaging;

namespace TelegramBotApp.Application.TelegramBotContext;

public interface IBotInitializer
{
    ITelegramBot CreateBot(string token);
    ReceiverOptions CreateReceiverOptions();
}

public interface ITelegramBot
{
    void StartReceiving(ReceiverOptions receiverOptions, IEventBus bus, CancellationToken cancellationToken);
    Task<User> GetMeAsync();
}