using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Application.Services;

[SuppressMessage("ReSharper", "AsyncVoidLambda")]
public class ResendMessageService(ITelegramBot bot, IEventBus bus) : IResendMessageService
{
    private readonly ConcurrentDictionary<long, List<ResendTimer>> _timers = new();

    public void AddOrUpdateResendProcess(long telegramId, string location, TimeSpan interval)
    {
        var timer = new Timer(async _ =>
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // TODO: add common settings
            var response = await bus.Publish(new WeatherForecastRequestIntegrationEvent(location),
                replyTo: nameof(UniversalResponse), cancellationToken: cts.Token);
            await bot.SendMessageAsync(telegramId, response.Message);
        }, null, interval, interval);

        _timers.AddOrUpdate(telegramId, _ => [new(timer, location)],
            (_, timers) =>
            {
                var resendTimer = timers.Find(x => x.Location == location);

                if (resendTimer != null)
                {
                    resendTimer.Timer.Dispose();
                    timers.Remove(resendTimer);
                }

                timers.Add(new(timer, location));
                return timers;
            });
    }

    public void RemoveResendProcess(long telegramId, string location)
    {
        if (!_timers.TryGetValue(telegramId, out var timers)) return;
        var timer = timers.FirstOrDefault(x => x.Location == location);
        timer?.Timer.Dispose();
        if (timer != null) timers.Remove(timer);
    }
}

internal record ResendTimer(Timer Timer, string Location);