using System.Text.Json;
using DatabaseApp.Application.Users.Queries.GetAllUsers;
using MediatR;
using TelegramBotApp.Domain.Responses;
using TelegramBotApp.Messaging.IntegrationContext;
using TelegramBotApp.Messaging.IntegrationContext.UserIntegrationEvents;

namespace DatabaseApp.IntegrationEvents.IntegrationEventHandlers;

public class GetAllUsersRequestIntegrationEventHandler(ISender mediator)
    : IIntegrationEventHandler<GetAllUsersRequestIntegrationEvent>
{
    public async Task<UniversalResponse?> Handle(GetAllUsersRequestIntegrationEvent @event)
    {
        var users = await mediator.Send(new GetAllUsersQuery());
        return new(JsonSerializer.Serialize(users.Select(u => u.TelegramId)));
    }
}