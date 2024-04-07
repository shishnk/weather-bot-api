using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

namespace TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponseHandlers;

public class AllUserResponseHandler(ICacheService cacheService) : IResponseHandler<AllUsersResponse>
{
    public async Task<UniversalResponse> Handle(AllUsersResponse response)
    {
        await cacheService.RemoveAsync("allUsers");

        await cacheService.SetAsync("allUsers", response.UserTelegramIds);

        return UniversalResponse.Empty;
    }
}