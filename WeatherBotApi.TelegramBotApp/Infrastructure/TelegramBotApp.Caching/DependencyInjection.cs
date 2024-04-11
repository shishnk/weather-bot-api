using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Caching.Caching;

namespace TelegramBotApp.Caching;

public static class DependencyInjection
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
                options.Configuration = configuration.GetConnectionString("Redis"))
            .AddSingleton<ICacheService, CacheService>();

        return services;
    }
}