using DatabaseApp.Application.UserWeatherSubscriptions.Commands.UpdateUserWeatherSubscription;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.Common;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserSubscriptionEventHandlers;

public class UpdatedUserSubscriptionIntegrationEventHandler(IServiceScopeFactory factory, ICacheService cacheService)
    : IIntegrationEventHandler<UpdatedUserSubscriptionIntegrationEvent>
{
    public async Task<IResponseMessage> Handle(UpdatedUserSubscriptionIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new UpdateUserWeatherSubscriptionCommand
        {
            Location = @event.Location,
            ResendInterval = @event.ResendInterval,
            UserTelegramId = @event.TelegramUserId
        });

        if (result.IsFailed) return new UniversalResponse(result.Errors.First().Message);

        var allSubscriptions = await cacheService.GetAsync<List<UserSubscriptionInfo>>("allSubscriptions");
        await cacheService.RemoveAsync("allSubscriptions");

        allSubscriptions ??= [];

        allSubscriptions.RemoveAll(x =>
            x.TelegramId == @event.TelegramUserId && x.Location == @event.Location);

        allSubscriptions.Add(new()
        {
            ResendInterval = @event.ResendInterval,
            Location = @event.Location,
            TelegramId = @event.TelegramUserId
        });
        await cacheService.SetAsync("allSubscriptions", allSubscriptions);

        return new UniversalResponse("Subscription updated successfully");
    }
}