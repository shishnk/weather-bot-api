using DatabaseApp.Application.Users.Commands.CreateUser;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers.UserIntegrationEventHandlers;

public class CreatedUserIntegrationEventHandler(IServiceScopeFactory factory, ICacheService cacheService)
    : IIntegrationEventHandler<CreatedUserIntegrationEvent>
{
    public async Task<IResponseMessage> Handle(CreatedUserIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new CreateUserCommand
        {
            TelegramId = @event.UserTelegramId,
            Username = @event.Username,
            MobileNumber = @event.MobileNumber,
            RegisteredAt = @event.RegisteredAt
        });

        if (!result.IsSuccess) return UniversalResponse.Empty;

        var allUsers = await cacheService.GetAsync<List<long>>("allUsers");
        await cacheService.RemoveAsync("allUsers");

        allUsers ??= [];
        allUsers.Add(@event.UserTelegramId);

        await cacheService.SetAsync("allUsers", allUsers);

        return UniversalResponse.Empty;
    }
}