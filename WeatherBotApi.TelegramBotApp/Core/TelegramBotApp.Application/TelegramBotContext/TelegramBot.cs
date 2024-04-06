using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBotApp.Application.Factories;
using TelegramBotApp.Messaging;

namespace TelegramBotApp.Application.TelegramBotContext;

public class TelegramBot(ITelegramBotClient telegramBot) : ITelegramBot
{
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(30);
    private const string HelpCommand = "/help";

    public void StartReceiving(ReceiverOptions receiverOptions, IEventBus bus, CancellationToken cancellationToken) =>
        telegramBot.StartReceiving(
            updateHandler: async (botClient, update, token) =>
                await HandleUpdateInnerAsync(botClient, update, bus, token),
            pollingErrorHandler: HandlePollingErrorInner,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken);

    public Task<User> GetMeAsync() => telegramBot.GetMeAsync();

    private async Task HandleUpdateInnerAsync(ITelegramBotClient botClient, Update update, IEventBus bus,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;
        if (message.Text is not { } messageText) return;

        var chatId = message.Chat.Id;
        using var cts = new CancellationTokenSource(_timeout);

        try
        {
            var args = messageText.Split(' ');
            var command = TelegramCommandFactory.GetCommand(args[0]);

            if (command.IsFailed)
            {
                await HandleError(botClient, chatId);
                return;
            }

            var response =
                await command.Value.Execute(args[0], args.Length > 1 ? args[1] : string.Empty, bus, cts.Token);

            if (response.IsFailed)
            {
                await HandleError(botClient, chatId);
                return;
            }

            await botClient.SendTextMessageAsync(chatId: chatId, response.Value,
                cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            await HandleError(botClient, chatId);
        }

        return;

        async Task HandleError(ITelegramBotClient bot, long chatIdInner)
        {
            var text = await TelegramCommandFactory
                .GetCommand(HelpCommand)
                .Value
                .Execute(HelpCommand, string.Empty, bus, CancellationToken.None);
            await bot.SendTextMessageAsync(chatId: chatIdInner, text.Value, cancellationToken: cancellationToken);
        }
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
}