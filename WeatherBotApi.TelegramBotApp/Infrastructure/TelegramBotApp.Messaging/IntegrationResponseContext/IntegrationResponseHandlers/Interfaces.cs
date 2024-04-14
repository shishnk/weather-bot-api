using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;

public interface IResponseHandler;

public interface IResponseHandler<in TRequest> : IResponseHandler
    where TRequest : IResponseMessage
{
    Task<UniversalResponse> Handle(TRequest response);
}