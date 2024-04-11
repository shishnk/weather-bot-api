using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;

public class UniversalResponseHandler : IResponseHandler<UniversalResponse>
{
    public Task<UniversalResponse> Handle(UniversalResponse response) => Task.FromResult(response);
}