using DatabaseApp.Application.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserSubscriptionEventHandlers;

public class CacheRequestUsersIntegrationEventHandler(IServiceScopeFactory factory, ICacheService cacheService)
    : IIntegrationEventHandler<CacheRequestUsersIntegrationEvent>
{
    public async Task<IResponseMessage> Handle(CacheRequestUsersIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var users = await mediator.Send(new GetAllUsersQuery());
        var telegramIds = users.Select(u => u.TelegramId).ToList();

        await cacheService.RemoveAsync("allUsers");
        await cacheService.SetAsync("allUsers", telegramIds);

        return UniversalResponse.Empty;
    }
}