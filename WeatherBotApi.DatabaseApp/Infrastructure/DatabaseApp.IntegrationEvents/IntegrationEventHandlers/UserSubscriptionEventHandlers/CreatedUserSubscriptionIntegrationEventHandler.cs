using DatabaseApp.Application.UserWeatherSubscriptions.Commands.CreateUserWeatherSubscription;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.Common;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserSubscriptionEventHandlers;

public class CreatedUserSubscriptionIntegrationEventHandler(IServiceScopeFactory factory, ICacheService cacheService)
    : IIntegrationEventHandler<CreatedUserSubscriptionIntegrationEvent>
{
    public async Task<IResponseMessage> Handle(CreatedUserSubscriptionIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new CreateUserWeatherSubscriptionCommand
        {
            Location = @event.City,
            ResendInterval = @event.ResendInterval,
            TelegramUserId = @event.TelegramId
        });

        if (result.IsFailed) return new UniversalResponse(result.Errors.First().Message);

        var allSubscriptions = await cacheService.GetAsync<List<UserSubscriptionInfo>>("allSubscriptions");
        await cacheService.RemoveAsync("allSubscriptions");

        allSubscriptions ??= [];

        allSubscriptions.Add(new()
        {
            ResendInterval = @event.ResendInterval,
            Location = @event.City,
            TelegramId = @event.TelegramId
        });
        await cacheService.SetAsync("allSubscriptions", allSubscriptions);

        return new UniversalResponse("Subscription created successfully");
    }
}