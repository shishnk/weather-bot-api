using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBotApp.Application;
using TelegramBotApp.Caching;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.Common;
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
                    .AddApplication(builder.Configuration)
                    .AddMessaging(builder.Configuration)
                    .AddResponseHandlers()
                    .AddCaching(builder.Configuration))
                .Build()
                .SubscribeToResponses();

            var cancellationTokenSource = new CancellationTokenSource();
            var eventBus = host.Services.GetRequiredService<IEventBus>();
            var botClient = host.Services.GetRequiredService<ITelegramBot>();
            var cacheService = host.Services.GetRequiredService<ICacheService>();
            var resendMessageService = host.Services.GetRequiredService<IResendMessageService>();

            await UpdateCache(eventBus);
            await InitializeResendMessageService(resendMessageService, cacheService);

            botClient.StartReceiving(
                eventBus,
                cacheService,
                resendMessageService,
                cancellationTokenSource.Token);

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

    private static async Task UpdateCache(IEventBus bus)
    {
        var task1 = bus.Publish(new CacheRequestUsersIntegrationEvent(),
            replyTo: nameof(UniversalResponse)); // TODO: fix cancellation token (common settings for bus), fix reply 
        var task2 = bus.Publish(new CacheRequestUserSubscriptionsIntegrationEvent(),
            replyTo: nameof(UniversalResponse));

        await Task.WhenAll(task1, task2);
    }

    private static async Task InitializeResendMessageService(IResendMessageService messageService,
        ICacheService cacheService)
    {
        var subscriptionInfos = await cacheService.GetAsync<List<UserSubscriptionInfo>>("allSubscriptions");
        if (subscriptionInfos?.Count == 0) return;
        subscriptionInfos?.ForEach(s =>
            messageService.AddOrUpdateResendProcess(s.TelegramId, s.Location, s.ResendInterval));
    }
}