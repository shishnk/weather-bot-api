using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging.RabbitMqProducer;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IMessageProducer, MessageProducer>();

var botClient = new TelegramBotClient("6819090366:AAEp-IrmVXY-U2Ie91lZktlkjxPG1IkJTJU");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

var provider = builder.Services.BuildServiceProvider();
var producer = provider.GetRequiredService<IMessageProducer>();
producer.EnsureInitialize();

botClient.StartReceiving(
    updateHandler: async (botClient, update, cancellationToken) =>
        await HandleUpdateAsync(botClient, update, cancellationToken),
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");

using var host = builder.Build();

await host.RunAsync();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClientInner, Update update,
    CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    await producer.PublishMessageAsync<WeatherRequest>(new()
    {
        Location = "Novosibirsk",
        User = "Kek"
    }, cts.Token);

    await botClientInner.SendTextMessageAsync(
        chatId: chatId,
        text: $"The temperature in Novosibirsk  C°",
        cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClientInner, Exception exception,
    CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}