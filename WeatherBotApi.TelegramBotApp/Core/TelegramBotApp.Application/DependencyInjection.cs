using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotApp.Application.Factories;
using TelegramBotApp.Application.Services;
using TelegramBotApp.Application.TelegramBotContext;
using TelegramBotApp.Domain.Models;

namespace TelegramBotApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITelegramBotSettings, TelegramBotSettings>();
        services.AddSingleton<TelegramCommandValidatorFactory>();
        services.AddSingleton<TelegramCommandFactory>();
        services.AddSingleton<ITelegramBotInitializer, TelegramBotInitializer>();
        services.AddSingleton<IResendMessageService, ResendMessageService>();

        services.AddSingleton<ITelegramBot>(s =>
        {
            var botInitializer = s.GetRequiredService<ITelegramBotInitializer>();
            return botInitializer.CreateBot(configuration.GetSection("TelegramSettings:BotToken").Value ??
                                            throw new InvalidOperationException("Bot token is not set."),
                botInitializer.CreateReceiverOptions());
        });

        return services;
    }
}