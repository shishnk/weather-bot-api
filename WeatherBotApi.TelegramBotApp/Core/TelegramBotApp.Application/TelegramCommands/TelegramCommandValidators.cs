using System.CommandLine.Parsing;
using System.Composition;
using FluentResults;

namespace TelegramBotApp.Application.TelegramCommands;

public class DefaultTelegramCommandValidator : ITelegramCommandValidator
{
    public string Command => string.Empty;

    public ParseArgument<Result> GetValidator() => _ => Result.Ok();
}

[Export(typeof(ITelegramCommandValidator))]
[ExportMetadata(nameof(Command), "/help")]
public class HelpTelegramCommandValidator : ITelegramCommandValidator
{
    public string Command => "/help";

    public ParseArgument<Result> GetValidator() =>
        _ => Result.Ok();
}

[Export(typeof(ITelegramCommandValidator))]
[ExportMetadata(nameof(Command), "/weather")]
public class WeatherTelegramCommandValidator : ITelegramCommandValidator
{
    public string Command => "/weather";

    public ParseArgument<Result> GetValidator() =>
        result => result.Tokens.Count == 0 ? Result.Fail("City name is required") : Result.Ok();
}