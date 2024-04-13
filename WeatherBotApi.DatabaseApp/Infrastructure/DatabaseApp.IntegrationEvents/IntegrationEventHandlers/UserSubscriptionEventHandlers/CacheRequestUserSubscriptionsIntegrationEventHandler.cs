using DatabaseApp.Application.Users.Queries.GetAllUsers;
using DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.Common;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserSubscriptionEventHandlers;

public class CacheRequestUserSubscriptionsIntegrationEventHandler(
    IServiceScopeFactory factory,
    ICacheService cacheService)
    : IIntegrationEventHandler<CacheRequestUserSubscriptionsIntegrationEvent>
{
    public async Task<IResponseMessage> Handle(CacheRequestUserSubscriptionsIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        List<UserSubscriptionInfo> subscriptionInfos = [];
        var userTelegramIds = await cacheService.GetAsync<List<long>>("allUsers");

        if (userTelegramIds is null)
        {
            var userDtos = await mediator.Send(new GetAllUsersQuery());
            userTelegramIds = userDtos.Select(dto => dto.TelegramId).ToList();
            await cacheService.SetAsync("allUsers", userTelegramIds);
        }

        foreach (var telegramId in userTelegramIds)
        {
            var userSubscription = await mediator.Send(new GetUserWeatherSubscriptionsQuery
            {
                UserTelegramId = telegramId
            });

            if (userSubscription.Count == 0) continue;

            subscriptionInfos.AddRange(userSubscription.Select(subscriptionDto => new UserSubscriptionInfo
            {
                TelegramId = subscriptionDto.UserTelegramId,
                ResendInterval = subscriptionDto.ResendInterval,
                Location = subscriptionDto.Location
            }));
        }

        await cacheService.RemoveAsync("allSubscriptions");
        await cacheService.SetAsync("allSubscriptions", subscriptionInfos);

        return UniversalResponse.Empty;
    }
}