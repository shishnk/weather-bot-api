using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBotApp.Application.TelegramBotContext;
using TelegramBotApp.Caching;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Api.AppPipeline;

public class AppPipeline : IPipeline
{
    public async Task Run()
    {
        try
        {
            using var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .ConfigureServices((builder, services) => services
                    .AddMessaging(builder.Configuration)
                    .AddResponseHandlers()
                    .AddStackExchangeRedisCache(options =>
                        options.Configuration = builder.Configuration.GetConnectionString("Redis"))
                    .AddCaching())
                .Build()
                .SubscribeToResponses();

            var botInitializer = new TelegramBotInitializer();
            var botClient = botInitializer.CreateBot(host.Services.GetRequiredService<IConfiguration>()
                                                         .GetRequiredSection("TelegramSettings:BotToken").Value ??
                                                     throw new InvalidOperationException("Bot token is not set."));
            var cancellationTokenSource = new CancellationTokenSource();
            var eventBus = host.Services.GetRequiredService<IEventBus>();

            UpdateCache(eventBus);

            botClient.StartReceiving(botInitializer.CreateReceiverOptions(),
                host.Services.GetRequiredService<IEventBus>(), cancellationTokenSource.Token);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            await cancellationTokenSource.CancelAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void UpdateCache(IEventBus bus) =>
        _ = bus.Publish(new GetAllUsersRequestIntegrationEvent(), replyTo: nameof(AllUsersResponse));
}