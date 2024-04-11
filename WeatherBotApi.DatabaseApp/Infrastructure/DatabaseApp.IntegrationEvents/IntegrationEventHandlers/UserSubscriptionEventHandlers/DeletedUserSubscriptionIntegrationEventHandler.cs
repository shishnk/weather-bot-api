using DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.Common;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserSubscriptionEventHandlers;

public class DeletedUserSubscriptionIntegrationEventHandler(IServiceScopeFactory factory, ICacheService cacheService)
    : IIntegrationEventHandler<DeletedUserSubscriptionIntegrationEvent>
{
    public async Task<IResponseMessage> Handle(DeletedUserSubscriptionIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var result = await mediator.Send(new DeleteUserWeatherSubscriptionCommand
        {
            UserTelegramId = @event.TelegramUserId,
            Location = @event.Location
        });

        if (result.IsFailed) return new UniversalResponse(result.Errors.First().Message);

        var allSubscriptions = await cacheService.GetAsync<List<UserSubscriptionInfo>>("allSubscriptions");
        await cacheService.RemoveAsync("allSubscriptions");

        allSubscriptions ??= [];

        allSubscriptions.RemoveAll(x =>
            x.TelegramId == @event.TelegramUserId && x.Location == @event.Location);

        await cacheService.SetAsync("allSubscriptions", allSubscriptions);

        return new UniversalResponse("Subscription deleted successfully");
    }
}