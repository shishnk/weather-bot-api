using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;

namespace TelegramBotApp.Caching;

public static class DependencyInjection
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}