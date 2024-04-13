using System.Composition;
using TelegramBotApp.Application.Factories.Common;
using TelegramBotApp.Application.TelegramCommands;

namespace TelegramBotApp.Application.Factories;

public class TelegramCommandValidatorFactory
{
    private class ImportInfo
    {
        [ImportMany]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public IEnumerable<Lazy<ITelegramCommandValidator, TelegramCommandMetadata>> Validators { get; set; } =
            Enumerable.Empty<Lazy<ITelegramCommandValidator, TelegramCommandMetadata>>();
    }

    private static readonly ImportInfo s_info = new();

    static TelegramCommandValidatorFactory() =>
        MefContainerConfiguration.SatisfyImports<ITelegramCommandValidator>(s_info);

    public static ITelegramCommandValidator GetValidatorForCommand(string command) =>
        s_info.Validators.FirstOrDefault(x => x.Metadata.Command == command)?.Value ??
        new DefaultTelegramCommandValidator();
}