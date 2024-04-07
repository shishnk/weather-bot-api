using DatabaseApp.Application.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers;

public class GetAllUsersRequestIntegrationEventHandler(IServiceScopeFactory factory)
    : IIntegrationEventHandler<GetAllUsersRequestIntegrationEvent>
{
    public async Task<IResponseMessage?> Handle(GetAllUsersRequestIntegrationEvent @event)
    {
        using var scope = factory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var users = await mediator.Send(new GetAllUsersQuery());
        return new AllUsersResponse { UserTelegramIds = users.Select(u => u.TelegramId).ToList() };
    }
}