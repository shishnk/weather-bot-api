using System.Composition;
using FluentResults;
using TelegramBotApp.Application.TelegramCommands;

namespace TelegramBotApp.Application.Factories;

public static class TelegramCommandFactory
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class TelegramCommandMetadata
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Command { get; set; } = string.Empty;
    }

    private class ImportInfo
    {
        [ImportMany]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public IEnumerable<Lazy<ITelegramCommand, TelegramCommandMetadata>> Commands { get; set; } =
            Enumerable.Empty<Lazy<ITelegramCommand, TelegramCommandMetadata>>();
    }

    private static readonly ImportInfo s_info = new();

    static TelegramCommandFactory() => MefContainerConfiguration.SatisfyImports<ITelegramCommand>(s_info);

    public static Result<ITelegramCommand> GetCommand(string command)
    {
        var commandMetadata = s_info.Commands.FirstOrDefault(x => x.Metadata.Command == command);
        return commandMetadata != null
            ? Result.Ok(commandMetadata.Value)
            : Result.Fail<ITelegramCommand>($"Command {command} not found");
    }
}