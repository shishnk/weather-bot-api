using MassTransit;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherApp.Domain.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(cfg => cfg.UsingRabbitMq());

var botClient = new TelegramBotClient("6819090366:AAEp-IrmVXY-U2Ie91lZktlkjxPG1IkJTJU");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

var bus = Bus.Factory.CreateUsingRabbitMq();
var client =
    bus.CreateRequestClient<TelegramBotWeatherRequest>(new("exchange:weather-forecast"),
        timeout: TimeSpan.FromSeconds(60));

botClient.StartReceiving(
    updateHandler: async (botClient, update, cancellationToken) =>
        await HandleUpdateAsync(botClient, update, client, cancellationToken),
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");

using var host = builder.Build();
await Task.WhenAll(bus.StartAsync(), host.RunAsync());

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
    IRequestClient<TelegramBotWeatherRequest> requestClient,
    CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    var response = await requestClient.GetResponse<WeatherDescriptor>(new()
    {
        Location = "Novosibirsk",
        User = "Kek"
    }, cancellationToken);

    Console.WriteLine(response.Message.Temperature);

    await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: $"The temperature in Novosibirsk {response.Message.Temperature} C°",
        cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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