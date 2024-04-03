using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
    .ConfigureServices((builder, services) => services.AddMessaging(builder.Configuration))
    .Build();

var botClient = new TelegramBotClient("6819090366:AAEp-IrmVXY-U2Ie91lZktlkjxPG1IkJTJU");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

var eventBus = host.Services.GetRequiredService<IEventBus>();
eventBus.SubscribeOnResponse<Kek>();
var kek = await eventBus.Publish(new RequestWeatherForecastIntegrationEvent(new("Novosibirsk")),
    nameof(Kek), CancellationToken.None);

Console.WriteLine(kek?.Value);
Console.ReadLine();

// botClient.StartReceiving(
//     updateHandler: async (botClient, update, cancellationToken) =>
//         await HandleUpdateAsync(botClient, update, cancellationToken),
//     pollingErrorHandler: HandlePollingErrorAsync,
//     receiverOptions: receiverOptions,
//     cancellationToken: cts.Token
// );

// var me = await botClient.GetMeAsync();
//
// Console.WriteLine($"Start listening for @{me.Username}");
// cts.Cancel();

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