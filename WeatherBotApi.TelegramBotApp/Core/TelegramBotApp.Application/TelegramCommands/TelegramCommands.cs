using System.Composition;
using FluentResults;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;

namespace TelegramBotApp.Application.TelegramCommands;

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/help")]
public class HelpTelegramCommand : ITelegramCommand
{
    public string Command => "/help";

    public Task<Result<string>> Execute(string command, string value, IEventBus bus,
        CancellationToken cancellationToken)
    {
        if (command != Command) throw new InvalidOperationException($"Invalid command {command}");

        return Task.FromResult(Result.Ok("""
                                         Hello! I'm a weather bot. I can provide you with the current weather in your city.
                                         You can use the following commands:
                                         /weather <city> - get the current weather in the specified city
                                         /help - get help
                                         """));
    }
}

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/weather")]
public class WeatherTelegramCommand : ITelegramCommand
{
    public string Command => "/weather";

    public async Task<Result<string>> Execute(string command, string value, IEventBus bus,
        CancellationToken cancellationToken)
    {
        if (command != Command) throw new InvalidOperationException($"Invalid command {command}");

        var response = await bus.Publish(new WeatherForecastRequestIntegrationEvent(value), ReplyNames.UniversalReply,
            cancellationToken);

        return response != null
            ? Result.Ok(response.Message)
            : Result.Fail("An error occurred while processing the request");
    }
}