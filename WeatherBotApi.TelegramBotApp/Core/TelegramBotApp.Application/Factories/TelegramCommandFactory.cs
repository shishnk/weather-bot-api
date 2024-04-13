using System.Collections.Concurrent;
using System.CommandLine;
using System.Composition;
using FluentResults;
using TelegramBotApp.Application.Factories.Common;
using TelegramBotApp.Application.Services;
using TelegramBotApp.Application.TelegramCommands;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;

namespace TelegramBotApp.Application.Factories;

public class TelegramCommandFactory
{
    private class ImportInfo
    {
        [ImportMany]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public ExportFactory<ITelegramCommand, TelegramCommandMetadata>[] Factories { get; set; } = [];
    }

    private static readonly ImportInfo s_info = new();
    private readonly RootCommand _rootCommand = new("Root command");
    private readonly Option<long> _globalOption;
    private readonly ConcurrentDictionary<object, Result<string>> _results = new();
    private readonly IEventBus _bus;
    private readonly ICacheService _cacheService;
    private readonly ITelegramBotSettings _settings;
    private readonly IResendMessageService _messageService;

    public TelegramCommandFactory(IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        ITelegramBotSettings settings)
    {
        MefContainerConfiguration.SatisfyImports<ITelegramCommand>(s_info);
        _bus = bus;
        _settings = settings;
        _cacheService = cacheService;
        _messageService = messageService;

        _globalOption = new(
            name: "--telegram-id",
            description: "Telegram chat id");

        _rootCommand.AddGlobalOption(_globalOption);

        foreach (var exportFactory in s_info.Factories)
        {
            _rootCommand.AddCommand(InitializeCommand(exportFactory.CreateExport().Value));
        }
    }

    public async Task<Result<string>> StartCommand(string args, long telegramId)
    {
        await _rootCommand.InvokeAsync($"{args} --telegram-id {telegramId}");
        _results.TryRemove(telegramId, out var result);
        return result ?? Result.Fail("Command not found or bad arguments");
    }

    private Command InitializeCommand(ITelegramCommand telegramCommand)
    {
        var command = new Command(telegramCommand.Command);
        var arguments = telegramCommand.Arguments.ToList();

        foreach (var argument in arguments)
        {
            // TODO: Uncomment when validators are implemented
            // argument.AddValidator(TelegramCommandValidatorFactory.GetValidatorForCommand(telegramCommand.Command).GetValidator());
            command.AddArgument(argument);
        }

        command.SetHandler(async context =>
        {
            var telegramId = context.ParseResult.GetValueForOption(_globalOption);
            var result = await telegramCommand.Execute(
                context,
                telegramId,
                GetValueForArgument,
                _bus,
                _cacheService,
                _messageService,
                _settings.Token);
            _results.TryAdd(telegramId, result.Value);
        });

        return command;

        Argument<string> GetValueForArgument(string name) =>
            arguments.FirstOrDefault(a => a.Name == name) ??
            throw new ArgumentException($"Argument not found with name '{name}'", nameof(name));
    }

    public static IEnumerable<string> GetCommands() =>
        s_info.Factories.Select(f => $"{f.Metadata.Command} {f.Metadata.Description}");
}