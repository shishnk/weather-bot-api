using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotApp.Application.Factories;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

namespace TelegramBotApp.Application.TelegramBotContext;

public class TelegramBot(ITelegramBotClient telegramBot, ReceiverOptions receiverOptions) : ITelegramBot
{
    private const string HelpCommand = "/help";

    private readonly ITelegramBotSettings _settings = TelegramBotSettings.CreateDefault();
    private TelegramCommandFactory _telegramCommandFactory = null!; // TODO: Refactor to use DI
    private ICacheService _cacheService = null!;

    public void StartReceiving(IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        _telegramCommandFactory = new(bus, cacheService, messageService, _settings);
        _cacheService = cacheService;
        
        telegramBot.StartReceiving(
            updateHandler: async (botClient, update, token) =>
                await HandleUpdateInnerAsync(botClient, update, bus, token),
            pollingErrorHandler: HandlePollingErrorInner,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken);
    }

    public Task SendMessageAsync(long telegramId, string message) =>
        telegramBot.SendTextMessageAsync(telegramId, message);

    public Task<User> GetMeAsync() => telegramBot.GetMeAsync();

    private Task HandleUpdateInnerAsync(ITelegramBotClient botClient, Update update, IEventBus bus, CancellationToken token)
    {
        if (update.Message is not { } message) return Task.CompletedTask;
        if (message.Text is not { } messageText) return Task.CompletedTask;

        var chatId = message.Chat.Id; // chat id equals to user telegram id

        Task.Run(async () =>
        {
            using var cts = new CancellationTokenSource(_settings.Timeout);

            try
            {
                _ = UpdateUsersCacheAsync(message, bus, cts.Token);
                var result = await _telegramCommandFactory.StartCommand(messageText, chatId);

                if (result.IsFailed)
                {
                    await HandleError(botClient, chatId, result);
                    return;
                }

                await botClient.SendTextMessageAsync(chatId: chatId, result.Value,
                    cancellationToken: cts.Token);
            }
            catch (TaskCanceledException)
            {
                await HandleError(botClient, chatId, Result.Fail("Timeout"));
            }
            catch (Exception)
            {
                await HandleError(botClient, chatId, result: Result.Fail("Internal error"));
            }
        }, token);
        return Task.CompletedTask;
    }

    private async Task HandleError(ITelegramBotClient bot, long chatIdInner, IResultBase result)
    {
        await bot.SendTextMessageAsync(chatId: chatIdInner, result.Errors.First().Message);
        var text = await _telegramCommandFactory.StartCommand(HelpCommand, chatIdInner);
        await bot.SendTextMessageAsync(chatId: chatIdInner, text.Value);
    }

    private static Task HandlePollingErrorInner(ITelegramBotClient botClientInner, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);

        return Task.CompletedTask;
    }

    private async Task UpdateUsersCacheAsync(Message message, IEventBus bus, CancellationToken cancellationToken)
    {
        var userTelegramIds = await _cacheService.GetAsync<List<long>>("allUsers", cancellationToken);

        if (userTelegramIds?.Contains(message.From!.Id) == false)
        {
            _ = await bus.Publish(new CreatedUserIntegrationEvent
            {
                MobileNumber = message.Contact?.PhoneNumber ?? "fake-number",
                Username = message.From?.Username ?? string.Empty,
                UserTelegramId = message.From?.Id ?? 0,
                RegisteredAt = DateTime.UtcNow
            }, cancellationToken: cancellationToken);
        }
    }
}