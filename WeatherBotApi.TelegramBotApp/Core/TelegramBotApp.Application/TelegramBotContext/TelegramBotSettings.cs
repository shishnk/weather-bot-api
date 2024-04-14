using TelegramBotApp.Domain.Models;

namespace TelegramBotApp.Application.TelegramBotContext;

public class TelegramBotSettings : ITelegramBotSettings
{
    private readonly CancellationTokenSource _cts;
    public TimeSpan Timeout => TimeSpan.FromSeconds(10);
    public CancellationToken Token => _cts.Token;

    private TelegramBotSettings() => _cts = new(Timeout);
    
    public static ITelegramBotSettings CreateDefault() => new TelegramBotSettings();
}