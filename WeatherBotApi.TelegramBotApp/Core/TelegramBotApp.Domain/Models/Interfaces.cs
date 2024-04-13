using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging;

namespace TelegramBotApp.Domain.Models;

public interface ITelegramBotSettings
{
    public TimeSpan Timeout { get; }
    public CancellationToken Token { get; }
}

public interface ITelegramBotInitializer
{
    ITelegramBot CreateBot(string token, ReceiverOptions receiverOptions);
    ReceiverOptions CreateReceiverOptions();
}

public interface ITelegramBot
{
    void StartReceiving(IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken);

    Task SendMessageAsync(long telegramId, string message);
    Task<User> GetMeAsync();
}

public interface IResendMessageService
{
    void AddOrUpdateResendProcess(long telegramId, string location, TimeSpan interval);
    void RemoveResendProcess(long telegramId, string location);
}